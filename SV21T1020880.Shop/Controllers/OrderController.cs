using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using SV21T1020880.Shop.AppCodes;
using SV21T1020880.Shop.Models;

namespace SV21T1020880.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        public IActionResult Index()
        {
            return View();
        }

        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, decimal salePrice, string productName)
        {
            if (salePrice <= 0 || quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == productId);

            if (existsProduct == null)
            {
                var newItem = new CartItem
                {
                    ProductID = productId,
                    Quantity = quantity,
                    SalePrice = salePrice,
                    ProductName = productName
                };
                shoppingCart.Add(newItem);
            }
            else
            {
                existsProduct.Quantity += quantity;
                existsProduct.SalePrice = salePrice;
            }

            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json(new { success = true, message = "Thêm hàng vào giỏ thành công." }); 
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int id)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);

            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json(""); // Thành công
        }

        // Xóa toàn bộ giỏ hàng
        [HttpGet]
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json(""); // Thành công
        }

        // Hiển thị giỏ hàng
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpGet]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var shoppingCart = GetShoppingCart();
            var item = shoppingCart.FirstOrDefault(m => m.ProductID == id);
            if (item != null)
            {
                item.Quantity = quantity;
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return Json(""); // Thành công
        }

        [HttpPost] 
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "") 
        { 
            try 
            { 
                var shoppingCart = GetShoppingCart(); 
                if (shoppingCart.Count == 0) 
                { 
                    TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán"; 
                    return RedirectToAction("Checkout"); 
                } 
                if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress)) 
                { 
                    TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng"; 
                    return RedirectToAction("Checkout"); 
                } 
                
                List<OrderDetail> orderDetails = new List<OrderDetail>(); 
                foreach (var item in shoppingCart) 
                { 
                    orderDetails.Add(new OrderDetail() 
                    { 
                        ProductID = item.ProductID, 
                        Quantity = item.Quantity, 
                        SalePrice = item.SalePrice 
                    }); 
                } 

                int orderID = OrderDataService.InitOrderCustomer(customerID, deliveryProvince, deliveryAddress, orderDetails); 
                if (orderID > 0) 
                { 
                    ClearCart(); 
                    TempData["SuccessMessage"] = "Đã đặt hàng thành công!";
                    return RedirectToAction("OrderDetails", new { id = orderID });
                } 
                TempData["ErrorMessage"] = "Không thể tạo đơn hàng. Vui lòng thử lại sau."; 
                return RedirectToAction("Checkout"); 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message); 
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình xử lý đơn hàng."; 
                return RedirectToAction("Checkout"); 
            } 
        }

        [HttpGet]
        public IActionResult Cancel(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại.");
                return View("OrderDetails", new { id });
            }

            // Cập nhật trạng thái đơn hàng thành "Canceled"
            bool success = OrderDataService.CancelOrder(id);
            if (!success)
            {
                ModelState.AddModelError("", "Không thể hủy đơn hàng.");
                return View("OrderDetails", new { id });
            }

            // Nếu thành công, quay lại trang Details
            return RedirectToAction("OrderDetails", new { id });
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = CommonDataService.GetCustomer(int.Parse(userData.UserId));

            var checkoutModel = new OrderCheckoutModel
            {
                CustomerID = int.Parse(userData.UserId),
                DeliveryProvince = customer.Province,
                DeliveryAddress = customer.Address,
                CartItems = GetShoppingCart()
            };

            return View(checkoutModel);
        }

        [HttpGet]
        public IActionResult OrderDetails(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("OrderList");
            }

            var orderDetailsModel = new OrderTrackingModel
            {
                OrderID = order.OrderID,
                OrderTime = order.OrderTime,
                CustomerName = order.CustomerName,
                DeliveryProvince = order.DeliveryProvince,
                DeliveryAddress = order.DeliveryAddress,
                OrderStatus = order.StatusDescription,
                OrderDetails = OrderDataService.ListOrderDetails(order.OrderID),
            };

            return View(orderDetailsModel);
        }

        [HttpGet]
        public IActionResult OrderList()
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customerID = int.Parse(userData.UserId);
            var orders = OrderDataService.GetOrdersByCustomer(customerID);

            return View(orders);
        }
    }
}

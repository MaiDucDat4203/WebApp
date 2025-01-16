using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using SV21T1020880.Web.AppCodes;
using SV21T1020880.Web.Models;

namespace SV21T1020880.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE},{WebUserRoles.ADMINISTRATOR}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };

            return View(condition);
        }
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(
                out rowCount,
                condition.Page,
                condition.PageSize,
                condition.SearchValue ?? "",
                condition.CategoryID,
                condition.SupplierID,
                condition.MinPrice,
                condition.MaxPrice
            );
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng mới";
            var data = new Product()
            {
                ProductID = 0,
                Photo = "NoImages.png"
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            if (string.IsNullOrEmpty(data.Photo))
                data.Photo = "NoImages.png";

            return View(data);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]//Attribute => Chỉ nhận dữ liệu gửi lên dưới dạng Post
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng mới" : "Cập nhật thông tin mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên không được để trống");

            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được để trống");

            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Tên nhà cung cấp không được để trống");

            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");

            if (data.Price < 0)
                ModelState.AddModelError(nameof(data.Price), "Vui lòng nhập giá lớn hơn 0");


            if (!ModelState.IsValid) 
            {

                return View("Edit", data);
            }
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
                return RedirectToAction("Edit", new { id });
            }
            else
            {
                ProductDataService.UpdateProduct(data);
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";

                    var data = new ProductPhoto()
                    {
                        PhotoID = 0,
                        DisplayOrder = 1,
                        ProductID = id,
                        IsHidden = false,
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Cập nhật ảnh cho mặt hàng";
                    var md = ProductDataService.GetPhoto(photoId);
                    if (md == null)
                        return RedirectToAction("Edit", new { id = id });
                    return View(md);
                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            if (uploadPhoto == null && data.PhotoID == 0)
                ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh của mặt hàng");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống");
            if (data.DisplayOrder == 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng chọn nhập thứ tự hiển thị");

            if (!ModelState.IsValid && data.PhotoID > 0)
            {
                ViewBag.Title = "Cập nhật ảnh cho mặt hàng";
                return View("Photo", data);
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                return View("Photo", data);
            }
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto?.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto?.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            if (data.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(data);
                if (id <= 0)
                {
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(data);
                if (!result)
                {
                    ViewBag.Title = "Cập nhật thông tin cho ảnh mặt hàng";
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (data.DisplayOrder == 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự không được để trống ");

            if (!ModelState.IsValid && data.AttributeID > 0) 
            {
                ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                return View("Attribute", data);
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Bổ sung thuộc tính của mặt hàng";
                return View("Attribute", data);
            }
            if (data.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(data);
                if (id <= 0)
                {
                    ViewBag.Title = "Bổ sung thuộc tính của mặt hàng";
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(data);
                if (!result)
                {
                    ViewBag.Title = "Cập nhật thông tin thuộc tính của ảnh mặt hàng";
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính của mặt hàng";

                    var data = new ProductAttribute()
                    {
                        AttributeID = 0,
                        DisplayOrder = 0,
                        ProductID = id
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính của mặt hàng";
                    var md = ProductDataService.GetAttribute(attributeId);
                    if (md == null)
                        return RedirectToAction("Edit");
                    return View(md);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}


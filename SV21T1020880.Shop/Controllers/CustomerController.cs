using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using System.Security.Claims;

namespace SV21T1020880.Shop.Controllers
{
    public class CustomerController : Controller
    {
        [HttpGet]
        public IActionResult Profile()
        {
            var userData = WebUserData.FromClaimsPrincipal(User);
            if (userData == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = CommonDataService.GetCustomer(int.Parse(userData.UserId));
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Index", "Home");
            }

            return View(customer);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var userData = WebUserData.FromClaimsPrincipal(User);
            if (userData == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customer = CommonDataService.GetCustomer(int.Parse(userData.UserId));
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin khách hàng.";
                return RedirectToAction("Profile");
            }

            return View(customer);
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Thông tin không hợp lệ. Vui lòng kiểm tra lại.");
                return View(data); // Trả về view với thông tin nhập sai
            }

            var userData = WebUserData.FromClaimsPrincipal(User);
            if (userData == null)
            {
                return RedirectToAction("Login", "Account");
            }

            data.CustomerId = int.Parse(userData.UserId); // Đảm bảo ID khớp với người dùng đã đăng nhập
            bool isUpdated = CommonDataService.UpdateCustomer(data);

            if (isUpdated)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError("", "Không thể cập nhật thông tin. Vui lòng thử lại.");
                return View(data);
            }
        }

    }
}

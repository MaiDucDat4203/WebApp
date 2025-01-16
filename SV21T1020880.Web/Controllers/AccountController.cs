using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using System.ComponentModel.DataAnnotations;

namespace SV21T1020880.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Trang đăng nhập vào hệ thống
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]        
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.Username = username;

            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }

            // TODO: Kiểm tra xem username và password có đúng hay không
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }


            // Đăng nhập thành công: Ghi nhận trạng thái đăng nhập
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };
            // 2. ghi nhan trang thai dang nhap
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            //3. Quay ve trang chu
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.Title = "Thay đổi mật khẩu";
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(string username, string oldPassword, string newPassword, string confirmPassword)
        {
            ViewBag.oldPassword = oldPassword;

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu mới và xác nhận mật khẩu không khớp";
                return View();
            }

            bool isChangePassword = UserAccountService.ChangePassword(UserTypes.Employee, username, oldPassword, newPassword);
            if (!isChangePassword)
            {
                ViewBag.Error = "Thay đổi mật khẩu thất bại";
                return View();
            }
            ViewBag.Success = "Mật khẩu đã được thay đổi thành công";
            return View();
        }

    }
}

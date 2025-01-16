using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;

namespace SV21T1020880.Shop.Controllers
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
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);
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
                DisplayName = userAccount.DisplayName
            };
            // 2. ghi nhan trang thai dang nhap
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            //3. Quay ve trang chu
            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.Title = "Thay đổi mật khẩu";
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult ChangePassword(string username = "", string oldpassword = "", string newpassword = "", string confirmPassword = "")
        {
            ViewBag.oldpassword = oldpassword;

            // Kiểm tra các thông tin nhập vào
            if (string.IsNullOrWhiteSpace(newpassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Mật khẩu mới và mật khẩu nhập lại không được để trống.";
                return View();
            }

            if (newpassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            // Gọi hàm thay đổi mật khẩu trong dịch vụ
            bool isChangePassword = UserAccountService.ChangePassword(UserTypes.Customer, username, oldpassword, newpassword);
            if (!isChangePassword)
            {
                ViewBag.Error = "Thay đổi mật khẩu thất bại. Kiểm tra lại mật khẩu cũ.";
                return View();
            }

            ViewBag.Success = "Mật khẩu đã được thay đổi thành công.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new Customer();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer customer, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu và xác nhận lại mật khẩu không khớp.";
                return View(customer);
            }

            var existingCustomer = CommonDataService.ListOfCustomers().FirstOrDefault(c => c.Email == customer.Email);
            if (existingCustomer != null)
            {
                ViewBag.Error = "Email đã tồn tại.";
                return View(customer);
            }

            customer.IsLocked = false;
            int newCustomerID = CommonDataService.AddCustomer(customer); ;

            if (newCustomerID > 0)
            {
                ViewBag.Success = "Đăng ký thành công!";
                return RedirectToAction("Register");
            }
            else
            {
                ViewBag.Error = "Đăng ký không thành công. Vui lòng thử lại.";
                return View(customer);
            }
        }

    }
}

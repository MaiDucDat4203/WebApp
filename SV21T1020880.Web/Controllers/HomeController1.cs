using Microsoft.AspNetCore.Mvc;

namespace SV21T1020880.Web.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

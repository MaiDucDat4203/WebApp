using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using SV21T1020880.Web.AppCodes;
using SV21T1020880.Web.Models;
using System.Buffers;

namespace SV21T1020880.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE},{WebUserRoles.ADMINISTRATOR}")]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";

        public IActionResult Index()
        {
            ViewBag.Title = "Quản lý khách hàng";

            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
            if (condition == null)
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }
  
        public IActionResult Search(PaginationSearchInput condition)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            CustomerSearchResult model = new CustomerSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);

            return View(model);
        }

        public IActionResult Create()
        {
            @ViewBag.Title = "Bổ sung khách hàng mới";
            var data = new Customer()
            {
                CustomerId = 0,
                IsLocked = false
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int Id = 0) 
        {
            @ViewBag.Title = "Cập nhật thông tin khách hàng";
            var data = CommonDataService.GetCustomer(Id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerId == 0 ? "Bổ sung khách hàng mới" : "Cập nhật thông tin khách hàng";
            // Kiểm tra nếu dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành cho khách hàng");

            // Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
            if (ModelState.IsValid == false) // !ModelState.IsValid
            {
                return View("Edit", data);
            }

            if (data.CustomerId == 0)
            {
                int id = CommonDataService.AddCustomer(data);
                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCustomer(data);
                if (result == false)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int Id) 
        {
            ViewBag.Title = "Xóa thông tin khách hàng";
            if (Request.Method == "POST") 
            { 
                CommonDataService.DeleteCustomer(Id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetCustomer(Id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);        
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using SV21T1020880.Web.AppCodes;
using SV21T1020880.Web.Models;
using System.Buffers;

namespace SV21T1020880.Web.Controllers
{
    [Authorize(Roles =$"{WebUserRoles.ADMINISTRATOR}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchSearchCondition"; 
        /// <summary>
        /// Hiển thị danh sách Employee
        /// </summary>
        /// <param name="page">Trang hiển thị</param>
        /// <param name="searchValue">Giá trị tìm kiếm</param>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.Title = "Quản lý nhân viên";

            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfEmployee(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");

            int pageCount = rowCount / PAGE_SIZE;
            if (rowCount % PAGE_SIZE > 0)
                pageCount += 1;
            EmployeeSearchResult model = new EmployeeSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
            return View(model);
        }

        /// <summary>
        /// Bổ sung nhân viên mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên mới";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true,
                Photo = "NoUser.jpg"
            };
            return View("Edit", data);
        }
        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IActionResult Edit(int Id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var data = CommonDataService.GetEmployee(Id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        /// <summary>
        /// Lưu thông tin nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <param name="_BirthDate"></param>
        /// <param name="_Photo"></param>
        /// <returns></returns>
        /// 
        [ValidateAntiForgeryToken]
        [HttpPost]        
        public IActionResult Save(Employee data, string _BirthDate, IFormFile? _Photo)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên mới" : "Cập nhật thông tin nhân viên";
            // Kiể tra nếu dữ liệu đầu vào không thể hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không thể để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại cho nhân viên");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ cho nhân viên");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email cho nhân viên");

            // Xử lý cho ngày sinh
            DateTime? d = _BirthDate.ToDateTime();
            if (d.HasValue) //(d !=null)
                data.BirthDate = d.Value;
            // Xử lý với ảnh
            if (_Photo != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                string FilePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    _Photo.CopyTo(stream);
                }
                data.Photo = fileName;
            }


            try
            {
                if (data.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn.");
                return View("Edit", data);
            }
        }
        /// <summary>
        /// Xóa nhân viên khỏi CSDL
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public IActionResult Delete(int Id)
        {
            ViewBag.Title = "Xóa thông tin nhân viên";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(Id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetEmployee(Id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}

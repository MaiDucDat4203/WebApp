﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using SV21T1020880.Web.AppCodes;
using SV21T1020880.Web.Models;

namespace SV21T1020880.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE},{WebUserRoles.ADMINISTRATOR}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 5;
        private const string CATEGORY_SEARCH_CONDITION = "CategorySearchCondition";
        public IActionResult Index()
        {
            ViewBag.Title = "Quản lý loại hàng";

            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_CONDITION);
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
            int rowCount;
            var data = CommonDataService.ListOfCategory(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");          
            CategorySearchResult model = new CategorySearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_CONDITION, condition);

            return View(model);
        }

        public IActionResult Create()
        {
            @ViewBag.Title = "Bổ sung loại hàng mới";
            var data = new Category()
            {
                CategoryID = 0,
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int Id = 0)
        {
            ViewBag.Title = "Chỉnh sửa thông tin loại hàng";
            var data = CommonDataService.GetCategory(Id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Save(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng mới" : "Cập nhật thông tin loại hàng";

            // Kiểm tra nếu dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Description))
                ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống");

            //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
            if (ModelState.IsValid == false) // !ModelState.IsValid
            {
                return View("Edit", data);
            }

            try
            {
                if (data.CategoryID == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng bị trùng");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }

            catch
            {
                ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            ViewBag.Title = "Xóa loại hàng";
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetCategory(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
    }
}
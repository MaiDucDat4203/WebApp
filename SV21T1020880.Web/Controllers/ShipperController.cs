﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.DomainModels;
using SV21T1020880.Web.AppCodes;
using SV21T1020880.Web.Models;
using System.Buffers;

namespace SV21T1020880.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.EMPLOYEE},{WebUserRoles.ADMINISTRATOR}")]
    public class ShipperController : Controller
    {
        public const int PAGE_SIZE = 5;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";

        public IActionResult Index()
        {
            ViewBag.Title = "Quản lý người giao hàng";
            PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
            var data = CommonDataService.ListOfShippers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            ShipperSearchResult model = new ShipperSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, condition);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng mới";
            var data = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng mới " : " Cập nhật thông tin người giao hàng";

            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên người giao hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");

            if (ModelState.IsValid == false)
            {
                return View("Edit", data);
            }

            try
            {
                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                        return View("Edit", data);
                    }

                }
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);
                    if (result == false)
                    {
                        ModelState.AddModelError(nameof(data.Phone), "Số điện thoại bị trùng");
                        return View("Edit", data);
                    }

                }
                return RedirectToAction("Index");

            }
            catch
            {
                ModelState.AddModelError("Erro", "Hệ thống tạm thời gián đoạn");
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SV21T1020880.BusinessLayers;
using SV21T1020880.Shop.AppCodes;
using SV21T1020880.Shop.Models;
using System.Diagnostics;


namespace SV21T1020880.Shop.Controllers
{
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 16;
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
        public IActionResult Details(int id)
        {
            var product = ProductDataService.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

    }
}

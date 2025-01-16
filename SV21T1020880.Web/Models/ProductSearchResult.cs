using SV21T1020880.DomainModels;

namespace SV21T1020880.Web.Models
{
    public class ProductSearchResult : PaginationSearchResult
    {
        public List<Product> Data { get; set; } = new List<Product>();
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    }
}

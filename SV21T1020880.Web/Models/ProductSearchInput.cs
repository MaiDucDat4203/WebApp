namespace SV21T1020880.Web.Models
{
    public class ProductSearchInput : PaginationSearchInput
    {
        public new int CategoryID { get; set; } = 0;
        public new int SupplierID { get; set; } = 0;
        public new decimal MinPrice { get; set; } = 0;
        public new decimal MaxPrice { get; set; } = 0;
    }
}

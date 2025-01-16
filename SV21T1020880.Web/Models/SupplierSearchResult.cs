using SV21T1020880.DomainModels;

namespace SV21T1020880.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }
    }
}

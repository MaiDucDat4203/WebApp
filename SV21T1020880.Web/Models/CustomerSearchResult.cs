using SV21T1020880.DomainModels;

namespace SV21T1020880.Web.Models
{
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }
    }
}

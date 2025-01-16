using SV21T1020880.DomainModels;

namespace SV21T1020880.Web.Models
{
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
}

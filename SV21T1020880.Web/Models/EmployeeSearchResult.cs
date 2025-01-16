using SV21T1020880.DomainModels;

namespace SV21T1020880.Web.Models
{
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
}

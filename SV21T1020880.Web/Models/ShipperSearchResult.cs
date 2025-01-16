using SV21T1020880.DomainModels;

namespace SV21T1020880.Web.Models
{
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }
    }
}

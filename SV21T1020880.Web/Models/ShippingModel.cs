using SV21T1020880.DomainModels;

namespace SV21T1020880.Web.Models
{
    public class ShippingModel
    {
        public int OrderID { get; set; }
        public int? CurrentShipperID { get; set; }
        public IEnumerable<Shipper> Shippers { get; set; }
    }
}

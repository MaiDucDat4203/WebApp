using SV21T1020880.DomainModels;

namespace SV21T1020880.Shop.Models
{
    public class OrderTrackingModel
    {
        public int OrderID { get; set; }
        public DateTime OrderTime { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryProvince { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderStatus { get; set; }
        public decimal Total { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}

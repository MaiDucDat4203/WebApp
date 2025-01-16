namespace SV21T1020880.Shop.Models
{
    public class OrderCheckoutModel
    {        
        public int CustomerID { get; set; }
        public string DeliveryProvince { get; set; } = "";
        public string DeliveryAddress { get; set; } = "";
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

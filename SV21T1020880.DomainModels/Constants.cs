namespace SV21T1020880.DomainModels
{
    /// <summary>
    /// Lớp Constants: khai báo các hằng biểu diễn cho các trạng thái của đơn hàng
    /// </summary>
    public static class Constants
    {
        public const int ORDER_INIT = 1;
        public const int ORDER_ACCEPTED = 2;
        public const int ORDER_SHIPPING = 3;
        public const int ORDER_FINISHED = 4;
        public const int ORDER_CANCEL = -1;
        public const int ORDER_REJECTED = -2;
    }
}

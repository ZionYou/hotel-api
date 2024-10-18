namespace HotelFull.Server.Models
{
    public class OrderItemRequest
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }

}

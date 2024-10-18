namespace HotelFull.Server.Models
{
    public class CreateOrderRequest
    {
        public int MemberID { get; set; }
        public string Status { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string? FrontDeskNotes { get; set; }
        public string? BackOfficeFeedback { get; set; }
        public List<ProductOrderItem> OrderItems { get; set; } = new();
    }

}

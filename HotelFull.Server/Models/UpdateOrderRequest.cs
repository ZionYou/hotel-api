namespace HotelFull.Server.Models
{
    public class UpdateOrderRequest
    {
        public string Status { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string? FrontDeskNotes { get; set; }
        public string? BackOfficeFeedback { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; } = new();
    }

}

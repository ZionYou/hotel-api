namespace HotelFull.Server.Models
{
    public class ProductOrderRequest
    {
        public int OrderID { get; set; }
        public int MemberID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public int ToatalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public string? FrontDeskNotes { get; set; }

        public string? BackOfficeFeedback { get; set; }
    }
}

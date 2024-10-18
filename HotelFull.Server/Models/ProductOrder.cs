using HotelAPI.Models;
using HotelFull.Server.Models;

public partial class ProductOrder
{
    public int OrderID { get; set; }  // 訂單ID

    public int MemberID { get; set; }  // 會員ID

    public DateTime OrderDate { get; set; }  // 訂單日期

    public OrderStatus Status { get; set; }  // 訂單狀態

    public PaymentStatus PaymentStat { get; set; }  // 付款狀態

    public string? FrontDeskNotes { get; set; }  // 前台備注

    public string? BackOfficeFeedback { get; set; }  // 後台反饋

    // 導航屬性，表示訂單中的商品項
    public virtual ICollection<ProductOrderItem> ProductOrderItem { get; set; } = new List<ProductOrderItem>();

    public virtual Member Member { get; set; } = null!;  // 會員導航屬性

    public enum OrderStatus
    {
        Pending = 1,     // 預訂中
        Confirmed = 2,   // 訂單成立
        Canceled = 3,    // 已取消訂單
        Completed = 4    // 已完成
    }

    public enum PaymentStatus
    {
        Unpaid = 1,      // 未付款
        Paid = 2,        // 已付款
        Refunded = 3     // 已退款
    }

}

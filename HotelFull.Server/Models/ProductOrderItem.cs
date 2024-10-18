using HotelAPI.Models;

namespace HotelFull.Server.Models
{
    public class ProductOrderItem
    {
        public int OrderID {  get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }


        public Product Product { get; set; } = null!;
        public ProductOrder ProductOrder { get; set; } = null!;
    }

}

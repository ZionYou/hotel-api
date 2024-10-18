using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Product
{
    public int ProductID { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public int? ProductTypeID { get; set; }

    public int? ImageID { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<ProductOrder> ProductOrder { get; set; } = new List<ProductOrder>();

    public virtual ProductType? ProductType { get; set; }
}

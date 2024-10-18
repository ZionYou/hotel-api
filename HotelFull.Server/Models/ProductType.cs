using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class ProductType
{
    public int ProductTypeID { get; set; }

    public string ProductTypeName { get; set; } = null!;

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}

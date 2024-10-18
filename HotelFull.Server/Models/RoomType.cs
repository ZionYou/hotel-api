using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class RoomType
{
    public int RoomTypeID { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public int MaxOccupancy { get; set; }

    public int? ImageID { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<Room> Room { get; set; } = new List<Room>();
}

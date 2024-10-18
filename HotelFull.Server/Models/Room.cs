using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Room
{
    public int RoomID { get; set; }

    public string RoomNumber { get; set; } = null!;

    public int Floor { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? LastCleaned { get; set; }

    public int RoomTypeID { get; set; }

    public int? ImageID { get; set; }

    public virtual Image? Image { get; set; }

    public virtual ICollection<RoomBooking> RoomBooking { get; set; } = new List<RoomBooking>();

    public virtual RoomType RoomType { get; set; } = null!;
}

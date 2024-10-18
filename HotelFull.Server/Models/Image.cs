using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Image
{
    public int ImageID { get; set; }

    public byte[] ImageBinary { get; set; } = null!;


    public int? ImageTypeID { get; set; }


    public virtual ICollection<Advertisement> Advertisement { get; set; } = new List<Advertisement>();

    public virtual ICollection<Employee> Employee { get; set; } = new List<Employee>();


    public virtual ImageType? ImageType { get; set; }

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();

    public virtual ICollection<Room> Room { get; set; } = new List<Room>();

    public virtual ICollection<RoomType> RoomType { get; set; } = new List<RoomType>();
}

using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class ImageType
{
    public int ImageTypeID { get; set; }

    public string ImageTypeName { get; set; } = null!;

    public virtual ICollection<Image> Image { get; set; } = new List<Image>();
}

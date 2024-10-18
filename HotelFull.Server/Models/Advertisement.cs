using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Advertisement
{
    public int AdvertisementID { get; set; }

    public string Title { get; set; } = null!;

    public int? ImageID { get; set; }

    public int? Adpriority { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
    public string? URL { get; set; }

}

using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class HotelInfo
{
    public int HotelInfoID { get; set; }

    public string HotelName { get; set; } = null!;

    public string FullAddress { get; set; } = null!;

    public decimal Longitude { get; set; } = 0.000000000000000M;

    public decimal Latitude { get; set; } = 0.000000000000000M;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Description { get; set; }

    public string? CheckInTime { get; set; }

    public string? CheckOutTime { get; set; }

   
}

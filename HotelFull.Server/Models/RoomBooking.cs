using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class RoomBooking
{
    public int BookingID { get; set; }

    public int MemberID { get; set; }

    public int RoomID { get; set; }

    public DateOnly CheckInDate { get; set; }

    public DateOnly CheckOutDate { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string? FrontDeskNotes { get; set; }

    public string? BackOfficeFeedback { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Employee
{
    public int EmployeeID { get; set; }

    public string Name { get; set; } = null!;

    public string IDNumber { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string? ClockInStatus { get; set; }

    public TimeOnly? BreakTime { get; set; }

    public string EmploymentStatus { get; set; } = null!;

    public int? ImageID { get; set; }

    public virtual Image? Image { get; set; }
}

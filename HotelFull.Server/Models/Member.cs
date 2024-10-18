using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class Member
{
    public int MemberID { get; set; }

    public string? Title { get; set; }

    public string? Name { get; set; } 

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? RefreshToken { get; set; } // 新增，存儲用戶的刷新令牌
    public DateTime? RefreshTokenExpiryTime { get; set; } // 新增，表示刷新令牌的過期時間

    public virtual ICollection<ProductOrder> ProductOrder { get; set; } = new List<ProductOrder>();

    public virtual ICollection<RoomBooking> RoomBooking { get; set; } = new List<RoomBooking>();
}
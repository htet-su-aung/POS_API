using System;
using System.Collections.Generic;

namespace DataModel;

public partial class TblMember
{
    public int MemberId { get; set; }

    public required string MemberCode { get; set; }

    public required string MemberName { get; set; }

    public required string Password { get; set; }

    public required string MobileNumber { get; set; }

    public required string Email { get; set; }

    public int TotalPoints { get; set; }

    public bool Active { get; set; }

    public string? OtpCode { get; set; }

    public bool Verified { get; set; }

    public virtual ICollection<TblExchangeCoupon> TblExchangeCoupons { get; set; } = new List<TblExchangeCoupon>();

    public virtual ICollection<TblPurchaseHistory> TblPurchaseHistories { get; set; } = new List<TblPurchaseHistory>();
}

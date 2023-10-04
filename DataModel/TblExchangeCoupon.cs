using System;
using System.Collections.Generic;

namespace DataModel;

public partial class TblExchangeCoupon
{
    public int ExchangeCouponId { get; set; }

    public int CouponId { get; set; }

    public int MemberId { get; set; }

    public bool IsUsed { get; set; }

    public virtual TblCoupon Coupon { get; set; }

    public virtual TblMember Member { get; set; }
}

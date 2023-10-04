using System;
using System.Collections.Generic;

namespace DataModel;

public partial class TblCoupon
{
    public int CouponId { get; set; }

    public string CouponName { get; set; }

    public string CouponNo { get; set; }

    public decimal Amount { get; set; }

    public int Points { get; set; }

    public int AvailableQty { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<TblExchangeCoupon> TblExchangeCoupons { get; set; } = new List<TblExchangeCoupon>();
}

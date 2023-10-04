using System;
using System.Collections.Generic;

namespace DataModel;

public partial class TblPurchaseHistory
{
    public int PurchaseHistoryId { get; set; }

    public string? InvoiceNo { get; set; }

    public decimal Amount { get; set; }

    public int? ExchangeCouponId { get; set; }

    public int MemberId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public virtual TblMember Member { get; set; }
}

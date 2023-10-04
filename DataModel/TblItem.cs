using System;
using System.Collections.Generic;

namespace DataModel;

public partial class TblItem
{
    public int ItemId { get; set; }

    public string ItemCode { get; set; }

    public string ItemName { get; set; }

    public decimal Price { get; set; }

    public bool Alcohol { get; set; }

    public bool Active { get; set; }
}

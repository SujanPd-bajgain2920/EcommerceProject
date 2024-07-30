using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class BillDetail
{
    public short Bdid { get; set; }

    public int? Bid { get; set; }

    public short? ProductId { get; set; }

    public decimal? Rate { get; set; }

    public int? Quantity { get; set; }

    public decimal? Amount { get; set; }

    public virtual BillRecord? BidNavigation { get; set; }

    public virtual Product? Product { get; set; }
}

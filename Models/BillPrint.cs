using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class BillPrint
{
    public short PrintId { get; set; }

    public int Bid { get; set; }

    public DateOnly PrintDate { get; set; }

    public short PrintBy { get; set; }

    public TimeOnly PrintTime { get; set; }

    public virtual BillRecord BidNavigation { get; set; } = null!;

    public virtual UserList PrintByNavigation { get; set; } = null!;
}

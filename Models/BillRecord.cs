using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class BillRecord
{
    public int Bid { get; set; }

    public int Bno { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public DateTime BillDate { get; set; }

    public string TransactionType { get; set; } = null!;

    public string? ReasonForCancel { get; set; }

    public DateTime CancelDate { get; set; }

    public short? CancelByUserId { get; set; }

    public short? EntryByUserId { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual ICollection<BillPrint> BillPrints { get; set; } = new List<BillPrint>();

    public virtual UserList? CancelByUser { get; set; }

    public virtual UserList? EntryByUser { get; set; }
}

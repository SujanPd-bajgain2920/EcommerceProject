using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class Product
{
    public short ProId { get; set; }

    public string ProName { get; set; } = null!;

    public decimal ProPrice { get; set; }

    public string Description { get; set; } = null!;

    public string ProImage { get; set; } = null!;

    public int OpeningQuantity { get; set; }

    public int SalesQuantity { get; set; }

    public int TotalQuantity { get; set; }

    public short CategoryId { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual Category? Category { get; set; }
}

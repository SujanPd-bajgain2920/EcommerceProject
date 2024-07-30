using System;
using System.Collections.Generic;

namespace InventoryManagementSystem.Models;

public partial class UserList
{
    public short UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string UsePhoto { get; set; } = null!;

    public string UserRole { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string CurrentAddress { get; set; } = null!;

    public virtual ICollection<BillPrint> BillPrints { get; set; } = new List<BillPrint>();

    public virtual ICollection<BillRecord> BillRecordCancelByUsers { get; set; } = new List<BillRecord>();

    public virtual ICollection<BillRecord> BillRecordEntryByUsers { get; set; } = new List<BillRecord>();
}

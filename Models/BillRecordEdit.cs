using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagementSystem.Models
{
    public class BillRecordEdit
    {
        public int Bid { get; set; }

        public int Userid { get; set; }

        public int BillNo { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime BillDate { get; set; }

        public string? TransactionType { get; set; }

        public string? ReasonforCancel { get; set; }


        public bool? Status { get; set; }

        public short Bdid { get; set; }

        public DateTime CancelDate { get; set; }

        public short? CancelByUserId { get; set; }

        public short? EntryByUserId { get; set; }

        public virtual List<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

        public virtual ICollection<BillPrint> BillPrints { get; set; } = new List<BillPrint>();

    }
}

using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class ProductEdit
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

        [DataType(DataType.Upload)]
        public IFormFile? ProductFile { get; set; } = null!;

        public string CatName { get; set; } = null!;

        public string EncId { get; set; } = null!;
    }
}

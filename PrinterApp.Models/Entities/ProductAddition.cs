using System.ComponentModel.DataAnnotations.Schema;

namespace PrinterApp.Models.Entities
{
    public class ProductAddition
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int ManufacturingAdditionId { get; set; }

        [ForeignKey("ManufacturingAdditionId")]
        public virtual ManufacturingAddition ManufacturingAddition { get; set; }

        public DateTime AddedDate { get; set; }
    }
}
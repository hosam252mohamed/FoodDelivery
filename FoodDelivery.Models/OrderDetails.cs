using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDelivery.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey("OrderHeader")]
        public int OrderHeaderId { get; set; }
        public OrderHeader OrderHeader { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Price { get; set; }

        // Add Quantity field
        [Required]
        public int Quantity { get; set; }  // Represents the number of units ordered for this product
    }
}

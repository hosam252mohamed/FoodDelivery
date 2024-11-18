using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string AppUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime OrderDate { get; set; }

        [ForeignKey("ShippingInfo")]
        public int shippingInfoId { get; set; }
        public ShippingInfo shippingInfo { get; set; }
        public decimal SubTotal { get; set; }
        public decimal OrderTotal { get; set; }
        public string CoupunType { get; set; }
        public string CoupunCode { get; set; }
		public decimal CoupunDiscount { get; set; }
        public string TransactionId { get; set; }
        public string SessionId { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
    }
}

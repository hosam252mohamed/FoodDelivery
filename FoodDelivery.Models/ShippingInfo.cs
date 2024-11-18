using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models
{
    public class ShippingInfo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string AppUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]

        public string Phone { get; set; }
        [Required]

        public string Government { get; set; }
        [Required]

        public string City { get; set; }

        public string PostalCode { get; set; }
        [Required]

        public string FullAddress { get; set; }
        public string NearestPlace { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models
{
    public class CustomerReview
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateEdited { get; set; }

        [ForeignKey("ApplicationUser")]
        public string AppUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

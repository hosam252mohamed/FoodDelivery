using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDelivery.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public DateTime DateAndTime { get; set; }

        [Required]
        public int NumberOfPeople { get; set; }

        public string SpecialRequest { get; set; }

        [Required]
        public string Status { get; set; } // "Pending", "Confirmed", "Canceled"

        // Reference to the user who made the booking
        [ForeignKey("ApplicationUser")]
        public string AppUserId { get; set; }
        public ApplicationUser AppUser { get; set; }
        public bool Confirmed { get; set; }
    }
}

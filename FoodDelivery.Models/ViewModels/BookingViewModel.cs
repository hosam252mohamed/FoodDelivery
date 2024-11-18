using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models.ViewModels
{
    public class BookingViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public int NumberOfPeople { get; set; }

        public string SpecialRequest { get; set; }

    }
}

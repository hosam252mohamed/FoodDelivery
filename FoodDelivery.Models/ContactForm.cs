using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery.Models
{
    public class ContactForm
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Not Valid Email Address")]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [MinLength(10, ErrorMessage = "Message must be a minimum length of 10")]
        [Required]
        public string Message { get; set; }
    }
}

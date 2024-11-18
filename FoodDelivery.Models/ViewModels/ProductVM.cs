using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FoodDelivery.Models.ViewModels
{
    public class ProductVM
    {
        [ValidateNever]
        public int ProductID { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public List<IFormFile> files { get; set; }
    }
}

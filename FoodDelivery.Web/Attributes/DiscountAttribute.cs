using FoodDelivery.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
    AllowMultiple = false)]
    public class DiscountAttribute : ValidationAttribute
    {
        private readonly ICoupunRepositoy _coupun;

        public DiscountAttribute(ICoupunRepositoy coupun)
        {
            _coupun = coupun;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return base.IsValid(value, validationContext);
        }
    }
}

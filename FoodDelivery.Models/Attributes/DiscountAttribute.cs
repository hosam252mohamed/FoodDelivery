using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.Models.Attributes
{
    public class DiscountAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var coupun = validationContext.ObjectInstance as Coupun;
            if (coupun.Type == "Percent")
            {
                if (coupun.Discount > 100 || coupun.Discount <= 0)
                {
                    return new ValidationResult(ErrorMessage = "Percent Should Be Between 1 and 100");
                }
            }
            else
            {
                if (coupun.Discount <= 0)
                {
                    return new ValidationResult(ErrorMessage = "Discount Cannot Be Negative");
                }
            }
            return ValidationResult.Success;
        }
    }
}
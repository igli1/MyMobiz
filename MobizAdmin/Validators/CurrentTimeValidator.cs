using System;
using System.ComponentModel.DataAnnotations;

namespace MobizAdmin.Validators
{
    public sealed class CurrentTimeValidator : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (Convert.ToDateTime(value) >= DateTime.Today)
                {
                
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Past dates not allowed!");
                }
            }
        }
}

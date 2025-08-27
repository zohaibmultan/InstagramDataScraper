#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaDataScraper.Common
{
    public class RequireAtLeastOneAttribute : ValidationAttribute
    {
        private readonly string[] _otherProperties;

        public RequireAtLeastOneAttribute(params string[] otherProperties)
        {
            _otherProperties = otherProperties;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isCurrentValueProvided = !string.IsNullOrWhiteSpace(value as string);

            bool isAnyOtherValueProvided = _otherProperties.Any(property =>
            {
                var otherValue = validationContext.ObjectInstance.GetType()
                    .GetProperty(property)?.GetValue(validationContext.ObjectInstance, null);
                return !string.IsNullOrWhiteSpace(otherValue as string);
            });

            if (!isCurrentValueProvided && !isAnyOtherValueProvided)
            {
                string errorMessage = ErrorMessage ?? $"At least one of {validationContext.DisplayName} or {string.Join(", ", _otherProperties)} must be provided.";
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }
    }
}

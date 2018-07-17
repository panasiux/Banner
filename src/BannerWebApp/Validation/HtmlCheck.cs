using System;
using System.ComponentModel.DataAnnotations;
using BannerWebApp.Models;
using Common;

namespace BannerWebApp.Validation
{
    public class HtmlCheck : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.ObjectInstance is BannerModel model))
                throw new ArgumentException("Attribute not applied on Employee");

            string htmlMessage;
            if (!HtmlFormatter.IsValidHtml(model.Html, out htmlMessage))
                return new ValidationResult($"html is invalid:{Environment.NewLine}{htmlMessage}");

            return ValidationResult.Success;
        }
    }
}

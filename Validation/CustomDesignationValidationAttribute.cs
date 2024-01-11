using System.ComponentModel.DataAnnotations;

public class CustomDesignationValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Your custom validation logic for Designation
        // Example: Check if the designation is one of a predefined set of values
        var validDesignations = new[] { "Manager", "Developer", "Analyst", "Senior Developer", "Senior Manager" };

        if (value != null && !validDesignations.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}

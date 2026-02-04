using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models;

public class CreateUserRequest : IValidatableObject

{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name?.Trim()))
        {
            yield return new ValidationResult("Name must contain non‑whitespace characters.", new[] { nameof(Name) });
        }
        if (string.IsNullOrWhiteSpace(Email?.Trim()))
        {
            yield return new ValidationResult("Email must not be empty or whitespace.", new[] { nameof(Email) });
        }
    }
}

// EmployeeSkillManagement.Models.Admin
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EmployeeSkillManagement.Models
{
    public class Admin : IdentityUser
    {
        // Additional properties can be added here

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(20, ErrorMessage = "Password cannot be more than 20 characters long.")]
        public string? PASSWORD { get; set; }
    }
}

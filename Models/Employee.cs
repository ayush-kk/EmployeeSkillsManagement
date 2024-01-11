using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeSkillManagement.Models
{
    public class Employee
    {
        [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name must not exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Invalid characters in First Name.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name must not exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Invalid characters in Last Name.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Date of Joining is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1900-01-01", "2100-12-31", ErrorMessage = "Date of Joining must be between 1900-01-01 and 2100-12-31.")]
        public DateTime DOJ { get; set; }


        [Required(ErrorMessage = "Designation is required.")]
        [CustomDesignationValidation(ErrorMessage = "Invalid Designation.")]
        public string? Designation { get; set; }


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]

        public string? Email { get; set; }


        // Navigation property for Skills
        public List<EmployeeSkill>? EmployeeSkills { get; set; }
    }
}

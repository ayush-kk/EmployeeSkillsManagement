using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeSkillManagement.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Skill Name is required.")]
        [StringLength(50, ErrorMessage = "Skill Name must not exceed 50 characters.")]
        public string? SkillName { get; set; }

        // Navigation property for Employees
        public List<EmployeeSkill>? EmployeeSkills { get; set; }
    }
}

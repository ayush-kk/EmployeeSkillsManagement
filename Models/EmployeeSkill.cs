using System.ComponentModel.DataAnnotations;

namespace EmployeeSkillManagement.Models
{
    public class EmployeeSkill
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int SkillId { get; set; }
        public Skill? Skill { get; set; }

        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public int Rating { get; set; }

        [Range(1, 20, ErrorMessage = "Years of Experience must be between 1 and 20.")]
        public int YearsOfExperience { get; set; }

        public bool IsPrimary { get; set; } // New property for skill type

        public List<Skill>? AvailableSkills { get; set; }
    }
}

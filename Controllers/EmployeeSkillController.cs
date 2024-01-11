using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeSkillManagement.Data;
using EmployeeSkillManagement.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeSkillManagement.Controllers
{
    [Authorize]
    public class EmployeeSkillController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize the database context
        public EmployeeSkillController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: EmployeeSkill/AssignSkill
        [HttpGet]
        public IActionResult AssignSkill(int employeeId)
        {
            // Retrieve the employee with associated skills
            var employee = _dbContext.Employees
                .Include(e => e.EmployeeSkills)
                .FirstOrDefault(e => e.Id == employeeId);

            if (employee == null)
            {
                return NotFound();
            }

            // Retrieve skills from the database
            var availableSkills = _dbContext.Skills.ToList();

            // Create the view model for assigning skills to the employee
            var viewModel = new EmployeeSkill
            {
                EmployeeId = employeeId,
                Employee = employee,
                Rating = 1,
                YearsOfExperience = 1,
                AvailableSkills = availableSkills ?? new List<Skill>(),
            };

            return View(viewModel);
        }

        // POST: EmployeeSkill/AssignSkill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignSkill(EmployeeSkill employeeSkill)
        {
            List<Skill> availableSkills;

            if (ModelState.IsValid)
            {
                // Set Skill property based on the selected value
                employeeSkill.Skill = _dbContext.Skills.Find(employeeSkill.SkillId);

                // Ensure Skill is not null
                if (employeeSkill.Skill == null)
                {
                    ModelState.AddModelError("SkillId", "Invalid Skill selected.");
                    availableSkills = _dbContext.Skills.ToList();
                    employeeSkill.AvailableSkills = availableSkills ?? new List<Skill>();

                    // Set TempData for error message
                    TempData["ToastrMessage"] = "Invalid Skill selected.";
                    TempData["ToastrType"] = "error";

                    return View(employeeSkill);
                }

                // Set Employee based on the EmployeeId
                employeeSkill.Employee = _dbContext.Employees.Find(employeeSkill.EmployeeId);

                // Ensure Employee is not null and set the EmployeeId
                if (employeeSkill.Employee != null)
                {
                    employeeSkill.EmployeeId = employeeSkill.Employee.Id;
                }
                else
                {
                    // Handle the case where the Employee is not found
                    ModelState.AddModelError("EmployeeId", $"Invalid EmployeeId selected: {employeeSkill.EmployeeId}");
                    availableSkills = _dbContext.Skills.ToList();
                    employeeSkill.AvailableSkills = availableSkills ?? new List<Skill>();

                    // Set TempData for error message
                    TempData["ToastrMessage"] = $"Invalid EmployeeId selected: {employeeSkill.EmployeeId}";
                    TempData["ToastrType"] = "error";

                    return View(employeeSkill);
                }

                // Add the employeeSkill to the context
                _dbContext.EmployeeSkills.Add(employeeSkill);
                _dbContext.SaveChanges();

                // Set TempData for success message
                TempData["ToastrMessage"] = "Skill assigned successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Index", "Employee");
            }

            // If the model state is not valid, redisplay the form with errors
            availableSkills = _dbContext.Skills.ToList();
            employeeSkill.AvailableSkills = availableSkills ?? new List<Skill>();

            // Set TempData for error message
            TempData["ToastrMessage"] = "Invalid data. Please check the form.";
            TempData["ToastrType"] = "error";

            return View(employeeSkill);
        }

        // GET: EmployeeSkill/ViewSkills
        public IActionResult ViewSkills(int employeeId)
        {
            // Retrieve the skills for the specified employeeId
            var skills = _dbContext.EmployeeSkills
                .Include(es => es.Skill)  // Include the Skill navigation property
                .Where(es => es.EmployeeId == employeeId)
                .ToList();

            return View(skills);
        }

        // POST: EmployeeSkill/DeleteSkill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSkill(int employeeId, int skillId)
        {
            // Retrieve the employee skill to delete
            var employeeSkill = _dbContext.EmployeeSkills
                .FirstOrDefault(es => es.EmployeeId == employeeId && es.SkillId == skillId);

            if (employeeSkill != null)
            {
                // Remove the employeeSkill from the context
                _dbContext.EmployeeSkills.Remove(employeeSkill);
                _dbContext.SaveChanges();

                // Set TempData for success message
                TempData["ToastrMessage"] = "Assigned Skill deleted successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Index", "Employee");
            }

            // Set TempData for error message
            TempData["ToastrMessage"] = "Skill not found or could not be deleted";
            TempData["ToastrType"] = "error";

            return RedirectToAction("ViewSkills", new { employeeId });
        }

    }
}

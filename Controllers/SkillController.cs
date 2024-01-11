using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeSkillManagement.Data;
using EmployeeSkillManagement.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeSkillManagement.Controllers
{
    [Authorize]
    public class SkillController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize the database context
        public SkillController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Skill/Index
        public IActionResult Index()
        {
            // Retrieve all skills from the database and display them
            var skills = _dbContext.Skills.ToList();
            return View(skills);
        }

        // GET: Skill/Create
        public IActionResult Create()
        {
            // Display the form to create a new skill
            return View();
        }

        // POST: Skill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SkillName")] Skill skill)
        {
            // Check if a skill with the same name already exists
            if (_dbContext.Skills.Any(s => s.SkillName.ToLower() == skill.SkillName.ToLower()))
            {
                ModelState.AddModelError("SkillName", "Skill with the same name already exists.");

                // Set TempData for error message
                TempData["ToastrMessage"] = "Error creating skill. Skill with the same name already exists.";
                TempData["ToastrType"] = "error";

                return View(skill);
            }

            if (ModelState.IsValid)
            {
                // Add the new skill to the database
                _dbContext.Add(skill);
                await _dbContext.SaveChangesAsync();

                // Set TempData for success message
                TempData["ToastrMessage"] = "Skill created successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }

            return View(skill);
        }

        // GET: Skill/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the skill by ID
            var skill = await _dbContext.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            // Display the form to edit an existing skill
            return View(skill);
        }

        // POST: Skill/Edit/5
        // POST: Skill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SkillName")] Skill skill)
        {
            if (id != skill.Id)
            {
                return NotFound();
            }

            // Check if a skill with the same name already exists, excluding the current skill
            if (_dbContext.Skills.Any(s => s.SkillName.ToLower() == skill.SkillName.ToLower() && s.Id != skill.Id))
            {
                ModelState.AddModelError("SkillName", "Skill with the same name already exists.");

                // Set TempData for error message
                TempData["ToastrMessage"] = "Error updating skill. Skill with the same name already exists.";
                TempData["ToastrType"] = "error";

                return View(skill);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the skill information in the database
                    _dbContext.Update(skill);
                    await _dbContext.SaveChangesAsync();

                    // Set TempData for success message
                    TempData["ToastrMessage"] = "Skill updated successfully";
                    TempData["ToastrType"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skill.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(skill);
        }


        // GET: Skill/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the skill by ID
            var skill = await _dbContext.Skills
                .FirstOrDefaultAsync(m => m.Id == id);

            if (skill == null)
            {
                return NotFound();
            }

            // Display the form to confirm deletion of a skill
            return View(skill);
        }

        // POST: Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the skill by ID
            var skill = await _dbContext.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            // Remove the skill from the database
            _dbContext.Skills.Remove(skill);
            await _dbContext.SaveChangesAsync();

            // Set TempData for success message
            TempData["ToastrMessage"] = "Skill deleted successfully";
            TempData["ToastrType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        // Check if a skill with a given ID exists
        private bool SkillExists(int id)
        {
            return _dbContext.Skills.Any(s => s.Id == id);
        }
    }
}

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
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize the database context
        public EmployeeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Employee/Index
        public IActionResult Index()
        {
            // Retrieve a list of employees and display them
            var employees = _dbContext.Employees.ToList();
            return View(employees);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            // Display the form to create a new employee
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,DOJ,Designation,Email")] Employee employee)
        {
            // Check if an employee with the same email already exists (case-insensitive)
            var existingEmployee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.Email.ToLower() == employee.Email.ToLower());

            if (existingEmployee != null)
            {
                // Add a toastr error message for duplicate email
                TempData["ToastrMessage"] = "Employee with the same email already exists.";
                TempData["ToastrType"] = "error";

                return View(employee);
            }

            if (ModelState.IsValid)
            {
                // Add the new employee to the database
                _dbContext.Add(employee);
                await _dbContext.SaveChangesAsync();

                // Set TempData for success message
                TempData["ToastrMessage"] = "Employee created successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }


        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the employee by ID
            var employee = await _dbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            // Display the form to edit an existing employee
            return View(employee);
        }

        // POST: Employee/Edit/5
        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,DOJ,Designation,Email")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            // Check if an employee with the same email already exists, excluding the current employee
            if (_dbContext.Employees.Any(e => e.Email.ToLower() == employee.Email.ToLower() && e.Id != employee.Id))
            {
                ModelState.AddModelError("Email", "Employee with the same email already exists.");

                // Set TempData for error message
                TempData["ToastrMessage"] = "Error updating employee. Employee with the same email already exists.";
                TempData["ToastrType"] = "error";

                return View(employee);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the employee information in the database
                    _dbContext.Update(employee);
                    await _dbContext.SaveChangesAsync();

                    // Set TempData for success message
                    TempData["ToastrMessage"] = "Employee updated successfully";
                    TempData["ToastrType"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }

            return View(employee);
        }


        // Check if an employee with a given ID exists
        private bool EmployeeExists(int id)
        {
            return _dbContext.Employees.Any(e => e.Id == id);
        }

        // GET: Employee/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            // Retrieve the employee by ID
            var employee = _dbContext.Employees.Find(id);

            if (employee == null)
            {
                // Handle the case where the employee is not found
                return NotFound();
            }

            // Display the form to confirm deletion of an employee
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _dbContext.Employees.Find(id);

            if (employee == null)
            {
                return NotFound();
            }

            // Remove the employee from the database
            _dbContext.Employees.Remove(employee);
            _dbContext.SaveChanges();

            // Set TempData for success message
            TempData["ToastrMessage"] = "Employee deleted successfully";
            TempData["ToastrType"] = "success";

            return RedirectToAction("Index");
        }
    }
}

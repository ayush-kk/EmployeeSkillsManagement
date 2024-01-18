using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EmployeeSkillManagement.Data;
using EmployeeSkillManagement.Models;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SignInManager<Admin> _signInManager;

    // Constructor to initialize dependencies
    public AdminController(ApplicationDbContext dbContext, SignInManager<Admin> signInManager)
    {
        _dbContext = dbContext;
        _signInManager = signInManager;
    }

    // Display a list of admins
    [HttpGet]
    public IActionResult Index()
    {
        var admins = _dbContext.Users.ToList();
        return View(admins);
    }

    // Display the form to create a new admin
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Handle the creation of a new admin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Admin model)
    {
        if (ModelState.IsValid)
        {
            // Validate email and password
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email is required.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.PASSWORD))
            {
                ModelState.AddModelError(nameof(model.PASSWORD), "Password is required.");
                return View(model);
            }

            // Set UserName to the provided email
            model.UserName = model.Email;

            // Create a new admin user
            _dbContext.Users.Add(model);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    // Display the form to edit an existing admin
    [HttpGet]
    public IActionResult Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var admin = _dbContext.Users.FirstOrDefault(a => a.Id == id);

        if (admin == null)
        {
            return NotFound();
        }

        return View(admin);
    }

    // Handle the edit of an existing admin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, Admin admin)
    {
        if (id != admin.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            // Update the admin information
            _dbContext.Update(admin);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        return View(admin);
    }

    // Display the form to confirm deletion of an admin
    [HttpGet]
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var admin = _dbContext.Users.FirstOrDefault(a => a.Id == id.ToString());

        if (admin == null)
        {
            return NotFound();
        }

        return View(admin);
    }

    // Handle the deletion of an admin
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var admin = _dbContext.Users.FirstOrDefault(a => a.Id == id.ToString());

        if (admin == null)
        {
            return NotFound();
        }

        // Remove the admin from the database
        _dbContext.Users.Remove(admin);
        _dbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    // Display the login form
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Handle the login attempt
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Admin model)
    {
        if (ModelState.IsValid)
        {
            // Attempt to sign in the admin using email and password
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.PASSWORD, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }

        // If the control reaches here, it means there was a validation error or login failure
        return View(model);
    }

    // Handle the logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Sign out the admin
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}

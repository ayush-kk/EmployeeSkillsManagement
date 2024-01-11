using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeSkillManagement.Data;
using EmployeeSkillManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace EmployeeSkillManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        // Constructor to initialize the database context
        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // GET: Home/Report
        [HttpGet]
        public IActionResult Report()
        {
            // Retrieve all employees for the report
            List<Employee> reportData = _dbContext.Employees.ToList();
            return View(reportData);
        }

        // GET: Home/ExportToExcel
        [HttpGet]
        public IActionResult ExportToExcel(string searchTerm, string searchCriteria)
        {
            List<Employee> reportData;

            if (string.IsNullOrEmpty(searchTerm))
            {
                // If no search term is provided, export all employees
                reportData = _dbContext.Employees
                .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill).ToList();
            }
            else
            {
                // Apply search criteria
                IQueryable<Employee> query = _dbContext.Employees
                    .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill);

                switch (searchCriteria)
                {
                    case "Name":
                        query = query.Where(e =>
                            EF.Functions.Like(e.FirstName, $"%{searchTerm}%") || EF.Functions.Like(e.LastName, $"%{searchTerm}%"));
                        break;

                    case "Skill":
                        query = query.Where(e => e.EmployeeSkills.Any(es => EF.Functions.Like(es.Skill.SkillName, $"%{searchTerm}%")));
                        break;

                    case "Designation":
                        query = query.Where(e => EF.Functions.Like(e.Designation, $"%{searchTerm}%"));
                        break;

                    default:
                        query = query.Where(e =>
                            EF.Functions.Like(e.FirstName, $"%{searchTerm}%") || EF.Functions.Like(e.LastName, $"%{searchTerm}%"));
                        break;
                }

                reportData = query.ToList();
            }

            // Check if there is data to export
            if (reportData.Count == 0)
            {
                // Return a message or handle the scenario where no data is found
                return Content("No data found for the specified criteria.");
            }

            // Create a new Excel package using EPPlus
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = excelPackage.Workbook.Worksheets.Add("EmployeeReport");

                // Create a header row
                int headerRowIndex = 1;
                foreach (var property in typeof(Employee).GetProperties())
                {
                    worksheet.Cells[headerRowIndex, property.GetIndex() + 1].Value = property.Name;
                }

                // Populate the sheet data with the report data
                int dataRowIndex = 2;
                foreach (var employee in reportData)
                {
                    foreach (var property in typeof(Employee).GetProperties())
                    {
                        if (property.PropertyType == typeof(List<EmployeeSkill>))
                        {
                            // If the property is a list of EmployeeSkill, get the skill names
                            var skillsList = (List<EmployeeSkill>)property.GetValue(employee);
                            var skillNames = skillsList?.Select(skill => skill.Skill?.SkillName).ToList();
                            worksheet.Cells[dataRowIndex, property.GetIndex() + 1].Value = string.Join(", ", skillNames) ?? "";
                        }
                        else
                        {
                            // For other properties, use the default ToString() representation
                            worksheet.Cells[dataRowIndex, property.GetIndex() + 1].Value = property.GetValue(employee)?.ToString() ?? "";
                        }
                    }
                    dataRowIndex++;
                }
                // Set the Content-Disposition header to suggest a filename
                var contentDisposition = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "EmployeeReport.xlsx"
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                // Return the Excel file as a downloadable file
                return File(excelPackage.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Home/Search
        [HttpPost]
        public IActionResult Search(string searchTerm, string searchCriteria)
        {
            try
            {
                // Your search logic here
                var employees = _dbContext.Employees
                    .Include(e => e.EmployeeSkills)
                    .ThenInclude(es => es.Skill)
                    .Where(e =>
                        (string.IsNullOrEmpty(searchCriteria) || searchCriteria == "Name") &&
                        (EF.Functions.Like(e.FirstName, $"%{searchTerm}%") || EF.Functions.Like(e.FirstName + " " + e.LastName, $"%{searchTerm}%") || EF.Functions.Like(e.FirstName + e.LastName, $"%{searchTerm}%") || EF.Functions.Like(e.LastName, $"%{searchTerm}%")) ||
                        (searchCriteria == "Skill" && e.EmployeeSkills.Any(es => EF.Functions.Like(es.Skill.SkillName, $"%{searchTerm}%"))) ||
                        (searchCriteria == "Designation" && EF.Functions.Like(e.Designation, $"%{searchTerm}%"))
                    )
                    .ToList();

                return PartialView("_SearchResults", employees);
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation (you might want to use a logging framework)
                Console.WriteLine(ex.Message);

                // Return a 500 status code with an error message
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}

// Extension class to get the index of a property in its declaring type
public static class PropertyInfoExtensions
{
    public static int GetIndex(this System.Reflection.PropertyInfo property)
    {
        return Array.IndexOf(property.DeclaringType.GetProperties(), property);
    }
}

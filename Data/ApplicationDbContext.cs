using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EmployeeSkillManagement.Models;

namespace EmployeeSkillManagement.Data
{
    // ApplicationDbContext is derived from IdentityDbContext, including the Admin user model
    public class ApplicationDbContext : IdentityDbContext<Admin>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSet properties for each entity in the database
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }

        // No need to declare DbSet<Admin> here as it's already included in the base class IdentityDbContext<Admin>

        // Override OnModelCreating to provide additional model configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base class implementation of OnModelCreating
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Employee and Skill using EmployeeSkill junction table
            modelBuilder.Entity<EmployeeSkill>()
                .HasKey(es => new { es.EmployeeId, es.SkillId });

            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Employee)
                .WithMany(e => e.EmployeeSkills)
                .HasForeignKey(es => es.EmployeeId);

            modelBuilder.Entity<EmployeeSkill>()
                .HasOne(es => es.Skill)
                .WithMany(s => s.EmployeeSkills)
                .HasForeignKey(es => es.SkillId);

            // Specify the primary key for the Admin entity
            modelBuilder.Entity<Admin>()
                .HasKey(a => a.Id);
        }
    }
}

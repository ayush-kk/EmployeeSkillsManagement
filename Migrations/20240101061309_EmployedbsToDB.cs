using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeSkillManagement.Migrations
{
    /// <inheritdoc />
    public partial class EmployedbsToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeSkillEmployeeId",
                table: "Skills",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeSkillSkillId",
                table: "Skills",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_EmployeeSkillEmployeeId_EmployeeSkillSkillId",
                table: "Skills",
                columns: new[] { "EmployeeSkillEmployeeId", "EmployeeSkillSkillId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_EmployeeSkills_EmployeeSkillEmployeeId_EmployeeSkillSkillId",
                table: "Skills",
                columns: new[] { "EmployeeSkillEmployeeId", "EmployeeSkillSkillId" },
                principalTable: "EmployeeSkills",
                principalColumns: new[] { "EmployeeId", "SkillId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skills_EmployeeSkills_EmployeeSkillEmployeeId_EmployeeSkillSkillId",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Skills_EmployeeSkillEmployeeId_EmployeeSkillSkillId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "EmployeeSkillEmployeeId",
                table: "Skills");

            migrationBuilder.DropColumn(
                name: "EmployeeSkillSkillId",
                table: "Skills");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_API.Migrations
{
    /// <inheritdoc />
    public partial class jkl2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "SalaryReports",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "SalaryReports");
        }
    }
}

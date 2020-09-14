using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyFinances.Migrations
{
    public partial class PaySourceGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Group",
                table: "PaySource",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Group",
                table: "PaySource");
        }
    }
}

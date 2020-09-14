using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyFinances.Migrations
{
    public partial class PaySourcesBalanceFieldAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "PaySource",
                type: "money",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "PaySource");
        }
    }
}

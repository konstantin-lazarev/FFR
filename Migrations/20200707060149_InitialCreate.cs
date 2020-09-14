using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyFinances.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaySource",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    FullName = table.Column<string>(maxLength: 50, nullable: true),
                    ValidThru = table.Column<string>(maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaySource", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseCategory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseCategory", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Sum = table.Column<decimal>(type: "money", nullable: false),
                    Purchase = table.Column<string>(nullable: true),
                    CategoryID = table.Column<int>(nullable: true),
                    PaySourceID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Expense_PurchaseCategory_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "PurchaseCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expense_PaySource_PaySourceID",
                        column: x => x.PaySourceID,
                        principalTable: "PaySource",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expense_CategoryID",
                table: "Expense",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_PaySourceID",
                table: "Expense",
                column: "PaySourceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "PurchaseCategory");

            migrationBuilder.DropTable(
                name: "PaySource");
        }
    }
}

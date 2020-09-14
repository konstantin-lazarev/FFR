using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyFinances.Migrations
{
    public partial class InpaymentsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inpayment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Sum = table.Column<decimal>(type: "money", nullable: false),
                    Income = table.Column<string>(nullable: true),
                    MonthlyIncome = table.Column<bool>(nullable: false),
                    PaySourceID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inpayment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Inpayment_PaySource_PaySourceID",
                        column: x => x.PaySourceID,
                        principalTable: "PaySource",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inpayment_PaySourceID",
                table: "Inpayment",
                column: "PaySourceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inpayment");
        }
    }
}

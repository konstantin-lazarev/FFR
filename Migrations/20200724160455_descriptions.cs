using Microsoft.EntityFrameworkCore.Migrations;

namespace FamilyFinances.Migrations
{
    public partial class descriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_PurchaseCategory_CategoryID",
                table: "Expense");

            migrationBuilder.DropForeignKey(
                name: "FK_Expense_PaySource_PaySourceID",
                table: "Expense");

            migrationBuilder.AlterColumn<int>(
                name: "PaySourceID",
                table: "Expense",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Expense",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_PurchaseCategory_CategoryID",
                table: "Expense",
                column: "CategoryID",
                principalTable: "PurchaseCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_PaySource_PaySourceID",
                table: "Expense",
                column: "PaySourceID",
                principalTable: "PaySource",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_PurchaseCategory_CategoryID",
                table: "Expense");

            migrationBuilder.DropForeignKey(
                name: "FK_Expense_PaySource_PaySourceID",
                table: "Expense");

            migrationBuilder.AlterColumn<int>(
                name: "PaySourceID",
                table: "Expense",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "Expense",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_PurchaseCategory_CategoryID",
                table: "Expense",
                column: "CategoryID",
                principalTable: "PurchaseCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_PaySource_PaySourceID",
                table: "Expense",
                column: "PaySourceID",
                principalTable: "PaySource",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

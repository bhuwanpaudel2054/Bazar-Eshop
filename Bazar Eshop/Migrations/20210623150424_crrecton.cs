using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazar_Eshop.Migrations
{
    public partial class crrecton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AspNetUsers_ApplicationUserId1",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ApplicationUserId1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Invoices");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Invoices",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ApplicationUserId",
                table: "Invoices",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AspNetUsers_ApplicationUserId",
                table: "Invoices",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AspNetUsers_ApplicationUserId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ApplicationUserId",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Invoices",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ApplicationUserId1",
                table: "Invoices",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AspNetUsers_ApplicationUserId1",
                table: "Invoices",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

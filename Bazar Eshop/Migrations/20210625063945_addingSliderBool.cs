using Microsoft.EntityFrameworkCore.Migrations;

namespace Bazar_Eshop.Migrations
{
    public partial class addingSliderBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Slider",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Slider");
        }
    }
}

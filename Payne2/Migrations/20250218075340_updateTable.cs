using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payne2.Migrations
{
    /// <inheritdoc />
    public partial class updateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sliders_Products_ProductId",
                table: "Sliders");

            migrationBuilder.DropIndex(
                name: "IX_Sliders_ProductId",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Sliders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Sliders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sliders_ProductId",
                table: "Sliders",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sliders_Products_ProductId",
                table: "Sliders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

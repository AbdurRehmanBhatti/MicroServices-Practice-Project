using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.ShoppingCartAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCardHeaderIdToCartHeaderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_CartHeaders_CardHeaderId",
                table: "CartDetails");

            migrationBuilder.DropIndex(
                name: "IX_CartDetails_CardHeaderId",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "CardHeaderId",
                table: "CartDetails");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartHeaderId",
                table: "CartDetails",
                column: "CartHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_CartHeaders_CartHeaderId",
                table: "CartDetails",
                column: "CartHeaderId",
                principalTable: "CartHeaders",
                principalColumn: "CartHeaderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_CartHeaders_CartHeaderId",
                table: "CartDetails");

            migrationBuilder.DropIndex(
                name: "IX_CartDetails_CartHeaderId",
                table: "CartDetails");

            migrationBuilder.AddColumn<int>(
                name: "CardHeaderId",
                table: "CartDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CardHeaderId",
                table: "CartDetails",
                column: "CardHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_CartHeaders_CardHeaderId",
                table: "CartDetails",
                column: "CardHeaderId",
                principalTable: "CartHeaders",
                principalColumn: "CartHeaderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

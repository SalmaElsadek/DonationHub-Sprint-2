using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonationHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonorUserId",
                table: "Donations",
                column: "DonorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_CreatedByUserId",
                table: "Campaign",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_AspNetUsers_CreatedByUserId",
                table: "Campaign",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_AspNetUsers_DonorUserId",
                table: "Donations",
                column: "DonorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_AspNetUsers_CreatedByUserId",
                table: "Campaign");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_AspNetUsers_DonorUserId",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_Donations_DonorUserId",
                table: "Donations");

            migrationBuilder.DropIndex(
                name: "IX_Campaign_CreatedByUserId",
                table: "Campaign");
        }
    }
}

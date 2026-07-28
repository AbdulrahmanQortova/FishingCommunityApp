using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishingCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFilteredUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TripReviews_TripId_UserId",
                table: "TripReviews");

            migrationBuilder.DropIndex(
                name: "IX_Boats_RegistrationNumber",
                table: "Boats");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TripReviews_TripId_UserId",
                table: "TripReviews",
                columns: new[] { "TripId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Boats_RegistrationNumber",
                table: "Boats",
                column: "RegistrationNumber",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TripReviews_TripId_UserId",
                table: "TripReviews");

            migrationBuilder.DropIndex(
                name: "IX_Boats_RegistrationNumber",
                table: "Boats");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TripReviews_TripId_UserId",
                table: "TripReviews",
                columns: new[] { "TripId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boats_RegistrationNumber",
                table: "Boats",
                column: "RegistrationNumber",
                unique: true);
        }
    }
}

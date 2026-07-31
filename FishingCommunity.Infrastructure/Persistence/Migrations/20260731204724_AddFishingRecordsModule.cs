using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishingCommunity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFishingRecordsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FishSpecies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FishSpecies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FishingLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FishSpeciesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeightKg = table.Column<double>(type: "float", nullable: true),
                    LengthCm = table.Column<double>(type: "float", nullable: true),
                    LocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    Bait = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CaughtDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeatherTemperature = table.Column<double>(type: "float", nullable: true),
                    WeatherDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhotoUrls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FishingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FishingLogs_FishSpecies_FishSpeciesId",
                        column: x => x.FishSpeciesId,
                        principalTable: "FishSpecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FishingLogs_CaughtDate",
                table: "FishingLogs",
                column: "CaughtDate");

            migrationBuilder.CreateIndex(
                name: "IX_FishingLogs_FishSpeciesId",
                table: "FishingLogs",
                column: "FishSpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_FishingLogs_Latitude_Longitude",
                table: "FishingLogs",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_FishingLogs_UserId",
                table: "FishingLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FishSpecies_Name",
                table: "FishSpecies",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FishingLogs");

            migrationBuilder.DropTable(
                name: "FishSpecies");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marcador.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    MatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SavedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Home_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Home_Score = table.Column<int>(type: "int", nullable: false),
                    Home_Fouls = table.Column<int>(type: "int", nullable: false),
                    Home_Timeouts30 = table.Column<int>(type: "int", nullable: false),
                    Home_Timeouts60 = table.Column<int>(type: "int", nullable: false),
                    Away_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Away_Score = table.Column<int>(type: "int", nullable: false),
                    Away_Fouls = table.Column<int>(type: "int", nullable: false),
                    Away_Timeouts30 = table.Column<int>(type: "int", nullable: false),
                    Away_Timeouts60 = table.Column<int>(type: "int", nullable: false),
                    Quarter = table.Column<int>(type: "int", nullable: false),
                    QuarterDurationMs = table.Column<int>(type: "int", nullable: false),
                    TimeLeftMs = table.Column<int>(type: "int", nullable: false),
                    Possession = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => new { x.MatchId, x.SavedAtUtc });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}

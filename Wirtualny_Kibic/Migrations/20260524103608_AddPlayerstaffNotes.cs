using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wirtualny_Kibic.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerstaffNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerSeasonStats");

            migrationBuilder.DropColumn(
                name: "ExternalApiPlayerId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "HeightCm",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PreferredFoot",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "Players");

            migrationBuilder.CreateTable(
                name: "PlayerStaffNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<int>(type: "INTEGER", nullable: false),
                    InjuryStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStaffNotes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStaffNotes");

            migrationBuilder.AddColumn<int>(
                name: "ExternalApiPlayerId",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeightCm",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredFoot",
                table: "Players",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeightKg",
                table: "Players",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlayerSeasonStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Appearances = table.Column<int>(type: "INTEGER", nullable: false),
                    Assists = table.Column<int>(type: "INTEGER", nullable: false),
                    DuelsWon = table.Column<int>(type: "INTEGER", nullable: false),
                    Goals = table.Column<int>(type: "INTEGER", nullable: false),
                    KeyPasses = table.Column<int>(type: "INTEGER", nullable: false),
                    League = table.Column<string>(type: "TEXT", nullable: false),
                    Lineups = table.Column<int>(type: "INTEGER", nullable: false),
                    Minutes = table.Column<int>(type: "INTEGER", nullable: false),
                    PassesTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<decimal>(type: "TEXT", nullable: true),
                    RedCards = table.Column<int>(type: "INTEGER", nullable: false),
                    Season = table.Column<int>(type: "INTEGER", nullable: false),
                    ShotsOnTarget = table.Column<int>(type: "INTEGER", nullable: false),
                    ShotsTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    Tackles = table.Column<int>(type: "INTEGER", nullable: false),
                    YellowCards = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSeasonStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerSeasonStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerSeasonStats_PlayerId",
                table: "PlayerSeasonStats",
                column: "PlayerId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wirtualny_Kibic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFormations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormationPlayers_Players_PlayerId",
                table: "FormationPlayers");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "ExternalPlayerId",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlayerName",
                table: "FormationPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlayerNumber",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerPhoto",
                table: "FormationPlayers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormationPlayers_Players_PlayerId",
                table: "FormationPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormationPlayers_Players_PlayerId",
                table: "FormationPlayers");

            migrationBuilder.DropColumn(
                name: "ExternalPlayerId",
                table: "FormationPlayers");

            migrationBuilder.DropColumn(
                name: "PlayerName",
                table: "FormationPlayers");

            migrationBuilder.DropColumn(
                name: "PlayerNumber",
                table: "FormationPlayers");

            migrationBuilder.DropColumn(
                name: "PlayerPhoto",
                table: "FormationPlayers");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormationPlayers_Players_PlayerId",
                table: "FormationPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

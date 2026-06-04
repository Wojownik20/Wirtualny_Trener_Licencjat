using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wirtualny_Kibic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFormations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormationPlayers_Players_PlayerId",
                table: "FormationPlayers");

            migrationBuilder.DropIndex(
                name: "IX_FormationPlayers_PlayerId",
                table: "FormationPlayers");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "FormationPlayers");

            migrationBuilder.RenameColumn(
                name: "RolePosition",
                table: "FormationPlayers",
                newName: "PositionLabel");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "FormationPlayers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ExternalPlayerId",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "SlotId",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "FormationPlayers");

            migrationBuilder.RenameColumn(
                name: "PositionLabel",
                table: "FormationPlayers",
                newName: "RolePosition");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "FormationPlayers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExternalPlayerId",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerId",
                table: "FormationPlayers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormationPlayers_PlayerId",
                table: "FormationPlayers",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormationPlayers_Players_PlayerId",
                table: "FormationPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");
        }
    }
}

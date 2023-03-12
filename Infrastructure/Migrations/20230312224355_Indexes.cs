using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Answers_OptionId",
                schema: "Tournament",
                table: "Answers");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                schema: "Tournament",
                table: "Participations",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Tournament",
                table: "Options",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "Tournament",
                table: "Options",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Tournament",
                table: "Channels",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participations_AccountId_ContestId_DrawnRank",
                schema: "Tournament",
                table: "Participations",
                columns: new[] { "AccountId", "ContestId", "DrawnRank" });

            migrationBuilder.CreateIndex(
                name: "IX_Options_Title_QuestionId_Text",
                schema: "Tournament",
                table: "Options",
                columns: new[] { "Title", "QuestionId", "Text" });

            migrationBuilder.CreateIndex(
                name: "IX_Contests_Title_Start_Finish_ChannelId_Calculated",
                schema: "Tournament",
                table: "Contests",
                columns: new[] { "Title", "Start", "Finish", "ChannelId", "Calculated" });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Title",
                schema: "Tournament",
                table: "Channels",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_OptionId_ParticipationId",
                schema: "Tournament",
                table: "Answers",
                columns: new[] { "OptionId", "ParticipationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participations_AccountId_ContestId_DrawnRank",
                schema: "Tournament",
                table: "Participations");

            migrationBuilder.DropIndex(
                name: "IX_Options_Title_QuestionId_Text",
                schema: "Tournament",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Contests_Title_Start_Finish_ChannelId_Calculated",
                schema: "Tournament",
                table: "Contests");

            migrationBuilder.DropIndex(
                name: "IX_Channels_Title",
                schema: "Tournament",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Answers_OptionId_ParticipationId",
                schema: "Tournament",
                table: "Answers");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                schema: "Tournament",
                table: "Participations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Tournament",
                table: "Options",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "Tournament",
                table: "Options",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "Tournament",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_OptionId",
                schema: "Tournament",
                table: "Answers",
                column: "OptionId");
        }
    }
}

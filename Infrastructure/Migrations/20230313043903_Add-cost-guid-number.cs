using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addcostguidnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.AddColumn<double>(
                name: "Cost",
                schema: "Tournament",
                table: "Contests",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                schema: "Tournament",
                table: "Contests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "guid",
                schema: "Tournament",
                table: "Contests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Contests_Title_Start_Finish_ChannelId_Calculated_guid",
                schema: "Tournament",
                table: "Contests",
                columns: new[] { "Title", "Start", "Finish", "ChannelId", "Calculated", "guid" });

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "Cost",
                schema: "Tournament",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "Number",
                schema: "Tournament",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "guid",
                schema: "Tournament",
                table: "Contests");

         
        }
    }
}

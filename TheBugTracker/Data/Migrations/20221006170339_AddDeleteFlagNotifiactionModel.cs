using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBugTracker.Data.Migrations
{
    public partial class AddDeleteFlagNotifiactionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeletedByRecipient",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeletedBySender",
                table: "Notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedByRecipient",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DeletedBySender",
                table: "Notifications");
        }
    }
}

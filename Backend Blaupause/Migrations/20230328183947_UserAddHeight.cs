using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Blaupause.Migrations
{
    public partial class UserAddHeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(name: "Height", table: "User", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Height", table: "User");
        }
    }
}

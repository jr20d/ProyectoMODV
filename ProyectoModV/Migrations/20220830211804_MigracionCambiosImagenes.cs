using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoModV.Migrations
{
    public partial class MigracionCambiosImagenes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "AspNetUsers",
                newName: "PublicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublicId",
                table: "AspNetUsers",
                newName: "AssetId");
        }
    }
}

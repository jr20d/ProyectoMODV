using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoModV.Migrations
{
    public partial class MigracionMarcas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Marca_MarcaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoVendedor_Productos_ProductoId",
                table: "ProductoVendedor");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoVendedor_Vendedores_VendedorId",
                table: "ProductoVendedor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductoVendedor",
                table: "ProductoVendedor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Marca",
                table: "Marca");

            migrationBuilder.RenameTable(
                name: "ProductoVendedor",
                newName: "ProductosVendedores");

            migrationBuilder.RenameTable(
                name: "Marca",
                newName: "Marcas");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoVendedor_VendedorId",
                table: "ProductosVendedores",
                newName: "IX_ProductosVendedores_VendedorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoVendedor_ProductoId",
                table: "ProductosVendedores",
                newName: "IX_ProductosVendedores_ProductoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductosVendedores",
                table: "ProductosVendedores",
                column: "ProductoVendedorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Marcas",
                table: "Marcas",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Marcas_MarcaId",
                table: "Productos",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "MarcaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductosVendedores_Productos_ProductoId",
                table: "ProductosVendedores",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductosVendedores_Vendedores_VendedorId",
                table: "ProductosVendedores",
                column: "VendedorId",
                principalTable: "Vendedores",
                principalColumn: "VendedorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Marcas_MarcaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductosVendedores_Productos_ProductoId",
                table: "ProductosVendedores");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductosVendedores_Vendedores_VendedorId",
                table: "ProductosVendedores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductosVendedores",
                table: "ProductosVendedores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Marcas",
                table: "Marcas");

            migrationBuilder.RenameTable(
                name: "ProductosVendedores",
                newName: "ProductoVendedor");

            migrationBuilder.RenameTable(
                name: "Marcas",
                newName: "Marca");

            migrationBuilder.RenameIndex(
                name: "IX_ProductosVendedores_VendedorId",
                table: "ProductoVendedor",
                newName: "IX_ProductoVendedor_VendedorId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductosVendedores_ProductoId",
                table: "ProductoVendedor",
                newName: "IX_ProductoVendedor_ProductoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductoVendedor",
                table: "ProductoVendedor",
                column: "ProductoVendedorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Marca",
                table: "Marca",
                column: "MarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Marca_MarcaId",
                table: "Productos",
                column: "MarcaId",
                principalTable: "Marca",
                principalColumn: "MarcaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoVendedor_Productos_ProductoId",
                table: "ProductoVendedor",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "ProductoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoVendedor_Vendedores_VendedorId",
                table: "ProductoVendedor",
                column: "VendedorId",
                principalTable: "Vendedores",
                principalColumn: "VendedorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    public partial class AutoresLibros : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comentarios_libros_LibroId",
                table: "comentarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_libros",
                table: "libros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comentarios",
                table: "comentarios");

            migrationBuilder.RenameTable(
                name: "libros",
                newName: "Libros");

            migrationBuilder.RenameTable(
                name: "comentarios",
                newName: "Comentarios");

            migrationBuilder.RenameIndex(
                name: "IX_comentarios_LibroId",
                table: "Comentarios",
                newName: "IX_Comentarios_LibroId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Libros",
                table: "Libros",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AutoresLibros",
                columns: table => new
                {
                    LibroId = table.Column<int>(type: "int", nullable: false),
                    AutorId = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoresLibros", x => new { x.AutorId, x.LibroId });
                    table.ForeignKey(
                        name: "FK_AutoresLibros_Autores_AutorId",
                        column: x => x.AutorId,
                        principalTable: "Autores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutoresLibros_Libros_LibroId",
                        column: x => x.LibroId,
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoresLibros_LibroId",
                table: "AutoresLibros",
                column: "LibroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Libros_LibroId",
                table: "Comentarios",
                column: "LibroId",
                principalTable: "Libros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Libros_LibroId",
                table: "Comentarios");

            migrationBuilder.DropTable(
                name: "AutoresLibros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Libros",
                table: "Libros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios");

            migrationBuilder.RenameTable(
                name: "Libros",
                newName: "libros");

            migrationBuilder.RenameTable(
                name: "Comentarios",
                newName: "comentarios");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_LibroId",
                table: "comentarios",
                newName: "IX_comentarios_LibroId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_libros",
                table: "libros",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comentarios",
                table: "comentarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comentarios_libros_LibroId",
                table: "comentarios",
                column: "LibroId",
                principalTable: "libros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

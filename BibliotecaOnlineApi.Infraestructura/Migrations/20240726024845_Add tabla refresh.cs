using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaOnlineApi.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class Addtablarefresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "REFRESH_TOKEN",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USER_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TOKEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JWT_ID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TOKEN_USADO = table.Column<bool>(type: "bit", nullable: false),
                    TOKEN_REVOCADO = table.Column<bool>(type: "bit", nullable: false),
                    FECHA_AGREGADO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FECHA_CADUCIDAD = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REFRESH_TOKEN", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REFRESH_TOKEN");

           
        }
    }
}

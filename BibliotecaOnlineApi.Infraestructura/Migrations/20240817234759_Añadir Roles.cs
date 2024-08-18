using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BibliotecaOnlineApi.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AñadirRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4781cb41-57c5-4902-b6d4-31ed47dd9c65", null, "Admin", "ADMIN" },
                    { "c893266a-ede5-4724-902b-cd11ef7dd559", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4781cb41-57c5-4902-b6d4-31ed47dd9c65");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c893266a-ede5-4724-902b-cd11ef7dd559");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ValueConversions.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Married = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Gender", "Gender2", "Married", "Name" },
                values: new object[,]
                {
                    { 1, "F", "Female", true, "Nunezsu" },
                    { 2, "M", "Male", false, "Salah" },
                    { 3, "F", "Female", true, "Diazgül" },
                    { 4, "M", "Male", false, "Fabinho" },
                    { 5, "F", "Female", true, "Van Dijknur" },
                    { 6, "M", "Male", true, "Henderson" },
                    { 7, "F", "Female", false, "Robertson Naz" },
                    { 8, "F", "Female", true, "Matipnur" },
                    { 9, "M", "Male", true, "Thiago" },
                    { 10, "M", "Male", true, "Alisson" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}

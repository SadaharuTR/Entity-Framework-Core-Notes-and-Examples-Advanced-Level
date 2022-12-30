using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SQLQueries.Migrations
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
                    PersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Name" },
                values: new object[,]
                {
                    { 1, "Teresa" },
                    { 2, "Michael" },
                    { 3, "Şükriye" },
                    { 4, "Süleyman" },
                    { 5, "Fatmanur" },
                    { 6, "Faruk" },
                    { 7, "Nalan" },
                    { 8, "Gül" },
                    { 9, "Bertunçcan" },
                    { 10, "Ayşenaz" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "Description", "PersonId" },
                values: new object[,]
                {
                    { 1, "...", 1 },
                    { 2, "...", 2 },
                    { 3, "...", 4 },
                    { 4, "...", 5 },
                    { 5, "...", 1 },
                    { 6, "...", 6 },
                    { 7, "...", 7 },
                    { 8, "...", 1 },
                    { 9, "...", 8 },
                    { 10, "...", 9 },
                    { 11, "...", 1 },
                    { 12, "...", 2 },
                    { 13, "...", 2 },
                    { 14, "...", 3 },
                    { 15, "...", 1 },
                    { 16, "...", 4 },
                    { 17, "...", 1 },
                    { 18, "...", 1 },
                    { 19, "...", 5 },
                    { 20, "...", 6 },
                    { 21, "...", 1 },
                    { 22, "...", 7 },
                    { 23, "...", 7 },
                    { 24, "...", 8 },
                    { 25, "...", 1 },
                    { 26, "...", 1 },
                    { 27, "...", 9 },
                    { 28, "...", 9 },
                    { 29, "...", 9 },
                    { 30, "...", 2 },
                    { 31, "...", 3 },
                    { 32, "...", 1 },
                    { 33, "...", 1 },
                    { 34, "...", 1 },
                    { 35, "...", 5 },
                    { 36, "...", 1 },
                    { 37, "...", 5 },
                    { 38, "...", 1 },
                    { 39, "...", 1 },
                    { 40, "...", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PersonId",
                table: "Orders",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}

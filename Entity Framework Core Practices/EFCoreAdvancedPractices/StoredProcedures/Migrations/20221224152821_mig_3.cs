using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoredProcedures.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        /// geriye değer döndüren SP
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                        CREATE PROCEDURE sp_BestSellingStaff
                        AS
	                        DECLARE @name NVARCHAR(MAX), @count INT
	                        SELECT TOP 1 @name = p.Name, @count = COUNT (*) FROM Persons P
	                        JOIN Orders o
		                        On p.PersonId = o.PersonId
	                        GROUP By p.Name
	                        ORDER By COUNT(*) DESC
	                        return @count");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP PROCEDURE sp_BestSellingStaff");
        }
    }
}

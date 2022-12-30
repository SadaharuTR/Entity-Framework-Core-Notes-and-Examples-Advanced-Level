
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScalarAndInlineFunctions.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        /// Id'sini vermiş olduğumuz personelin yapmış olduğu satışların toplam fiyat bilgisini dönen bir fonksiyon.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                        CREATE FUNCTION getPersonTotalOrderPrice(@personId INT)
	                        RETURNS INT
                        AS
                        BEGIN
	                        DECLARE @totalPrice INT
	                        SELECT @totalPrice = SUM(o.Price) FROM Persons p
	                        JOIN Orders o
		                        ON p.PersonId = o.PersonId
	                        WHERE p.PersonId = @personId
	                        RETURN @totalPrice
                        END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP FUNCTION getPersonTotalOrderPrice");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeylessEntityTypes.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        //bu migration'u migrate ettiğimizde veritabanında vw_PersonOrderCount adında bir view oluşturulacak ve bu view'in içerisinde aşağıdaki herhangi bir PK'ye
        //sahipsorgu elde edilip geriye sonucu döndürmüş olacaktır.
        {
            migrationBuilder.Sql($@"
                        CREATE VIEW vw_PersonOrderCount
                        AS
	                        select p.Name, COUNT(*) [Count] FROM Persons p
	                        JOIN Orders o
		                        ON p.PersonId = o.PersonId
	                        GROUP By p.Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"DROP VIEW vw_PersonOrderCount");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Value_Conversions.Configurations;

public class PersonData : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasData(
                 new() { Id = 1, Name = "Nunezsu", Gender = "F", Gender2 = Gender.Female, Married = true },
                 new() { Id = 2, Name = "Salah", Gender = "M", Gender2 = Gender.Male, Married = false },
                 new() { Id = 3, Name = "Diazgül", Gender = "F", Gender2 = Gender.Female, Married = true },
                 new() { Id = 4, Name = "Fabinho", Gender = "M", Gender2 = Gender.Male, Married = false },
                 new() { Id = 5, Name = "Van Dijknur", Gender = "F", Gender2 = Gender.Female, Married = true },
                 new() { Id = 6, Name = "Henderson", Gender = "M", Gender2 = Gender.Male, Married = true },
                 new() { Id = 7, Name = "Robertson Naz", Gender = "F", Gender2 = Gender.Female, Married = false },
                 new() { Id = 8, Name = "Matipnur", Gender = "F", Gender2 = Gender.Female, Married = true },
                 new() { Id = 9, Name = "Thiago", Gender = "M", Gender2 = Gender.Male, Married = true },
                 new() { Id = 10, Name = "Alisson", Gender = "M", Gender2 = Gender.Male, Married = true }
            );
    }
}
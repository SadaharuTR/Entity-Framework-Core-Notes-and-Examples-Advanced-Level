using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectionResiliency.Configurations;

public class PersonData : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasData(new Person[]
        {
              new(){ PersonId  = 1, Name = "A101" },
              new(){ PersonId  = 2, Name = "Migros" },
              new(){ PersonId  = 3, Name = "Şenol" },
              new(){ PersonId  = 4, Name = "Güneş" },
              new(){ PersonId  = 5, Name = "Alvaro" },
              new(){ PersonId  = 6, Name = "Sanchez" },
              new(){ PersonId  = 7, Name = "Gülüm" },
              new(){ PersonId  = 8, Name = "Jale" },
              new(){ PersonId  = 9, Name = "Ramiz" },
              new(){ PersonId  = 10, Name = "Dayı" },
        });
    }
}
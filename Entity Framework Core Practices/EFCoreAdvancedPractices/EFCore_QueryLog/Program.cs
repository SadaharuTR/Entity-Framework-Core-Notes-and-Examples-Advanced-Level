
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();
#region Query Log Nedir?
//LINQ sorguları neticesinde generate edilen sorguları izleyebilmek ve olası teknik hataları ayıklayabilmek amacıyla query log mekanizmasından istifade ederiz.
//SQL Profiler'da aynı işlemi yapabiliyoruz. Ama bazı veritabanlarına Profiler olmayabilir. EF Core'un desteklemiş olduğu her veritabanına karşılık bir sorgu görselleştirici
//kullanacağımıza buradaki sorguları merkezi bir noktaya loglayarak daha performanslı, hızlı bir davranış sergileyebiliriz.
#endregion
#region Nasıl Konfigüre Edilir?
//Microsoft.Extensions.Logging.Console kütüphanesi ile yapmış olduğumuz çalışmaların Query'leri konsolda loglayabiliriz.
//Microsoft.Extensions.Logging yazarak NuGet'te diğer muadil ortamlar için kütüphanelerde mevcut.
//Microsoft.Extensions.Logging.EventLog, Microsoft.Extensions.Logging.AzureAppServices, Microsoft.Extensions.Logging.Debug ........

await context.Persons.ToListAsync(); //sonucunda ekrana,
/* 
 info: Microsoft.EntityFrameworkCore.Database.Command[20101]
       Executed DbCommand (48ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
       SELECT [p].[PersonId], [p].[Name]
       FROM [Persons] AS [p]
*/ //yazacaktır.

//Sorguyu daha kompleks bir hale getirirsek,
await context.Persons
     .Include(p => p.Orders)
     .Where(p => p.Name.Contains("a"))
     .Select(p => new { p.Name, p.PersonId })
     .ToListAsync();
/*
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (47ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      SELECT [p].[Name], [p].[PersonId]
      FROM [Persons] AS [p]
      WHERE [p].[Name] LIKE N'%a%' 
*/ //şeklinde ekrana log'layacaktır.
#endregion

#region Filtreleme Nasıl Yapılır?
//LoggerFactory üzerinden AddFilter ile.
#endregion

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public ICollection<Order> Orders { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    public Person Person { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Orders)
            .WithOne(o => o.Person)
            .HasForeignKey(o => o.PersonId);
    }
    /*
    //İlk adımda, eklemiş olduğumuz kütüphaneye karşılık bir LoggerFactory oluştururuz.
    readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder  => builder.AddConsole());
    //Microsoft.Extensions.Logging.Console kütüphanesi sayesinde AddConsole()'u kullanabiliyoruz.
    */

    //Filtre ekleme işlemleri;
    
    readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder
    .AddFilter((category, level) =>
    {
        return category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information; //ile Information seviyesinde loglar ile filtrelemiş oluruz.
    })
    .AddConsole());
    
    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");
        //İkinci adımda ise UseLoggerFactory ile sistemde devreye sokuyoruz.
        optionsBuilder.UseLoggerFactory(loggerFactory);
        //bundan sonra yapmış olduğumuz çalışmalarda generate dilen bütün sorguların hepsi ilgili ortama /burada konsola/ loglanacaktır.
    }
}
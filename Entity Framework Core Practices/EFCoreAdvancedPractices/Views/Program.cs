using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region View Nedir?
//Oluşturduğumuz kompleks sorguları ihtiyaç durumlarında daha rahat bir şekilde kullanabilmek için
//basitleştiren bir veritabanı objesidir.
#endregion

#region EF Core İle View Kullanımı

#region View Oluşturma
//1. adım : Boş bir migration oluşturulmalıdır.
//2. adım : Migration içerisindeki Up fonksiyonunda View'in create komutları, Down fonksiyonunda ise
//drop komutları yazılmalıdır.
//3. adım : Migrate ediniz.
#endregion

#region View'i DbSet Olarak Ayarlama
//View'i EF Core üzerinden sorgulayabilmek için View neticesini karşılayabilecek bir entity olşturulması
//ve bu entity türünden DbSet property'sinin eklenmesi gerekmektedir.
#endregion

#region DbSet'in Bir View Olduğunu Bildirmek
//OnModelCreating'i kurcalamak lazım.
#endregion

//View'i sorgulayalım.
//var personOrders = await context.PersonOrders.ToListAsync();

//View üzerinde Linq sorgularını da kullanabiliriz.
/*
var personOrders = await context.PersonOrders
    .Where(po => po.Count > 10)
    .ToListAsync();
*/

#region EF Core'da View'lerin Özellikleri
//View'lerde Primary Key olmaz! Bu yüzden ilgili DbSet'in HasNoKey ile işaretlenmesi gerekmektedir.

//View neticesinde gelen veriler Change Tracker ile takip edilmezler. Haliyle üzerlerinde yapılan
//değişiklikleri EF Core veritabanına yansıtmaz.
/*
var personOrder = await context.PersonOrders.FirstAsync();
personOrder.Name = "Tsubasa"; //veritabanına yansımaz.
await context.SaveChangesAsync();
*/
#endregion
Console.WriteLine("");
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

    public Person Person { get; set; }
}
public class PersonOrder //içinde Name ve Count'u karşılayabilecek property'ler olmalıdır.
{
    public string Name { get; set; }
    public int Count { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PersonOrder> PersonOrders { get; set; } //view'i temsil eden entity'i DbSet olarak ekleyelim.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //Bir View olduğunu EF Core'a bildirmemiz lazım.
        modelBuilder.Entity<PersonOrder>()
            .ToView("vm_PersonOrders") //View'in adını bildirelim.
            .HasNoKey(); //View'in PK'i olmadığını bildirmemiz de lazım.

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Orders)
            .WithOne(o => o.Person)
            .HasForeignKey(o => o.PersonId);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");
    }
}
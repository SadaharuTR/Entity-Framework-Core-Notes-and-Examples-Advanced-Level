using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Table Per Concrete Type (TPC) Nedir?
//TPC davranışı, kalıtımsal ilişkiye sahip olan Entity'lerin olduğu çalışmalarda sadece concrete/somut olan Entity'lere karşılık bir tablo oluşturacak bir davranış modelidir.
//TPC, TPT'nin daha performanslı versiyonudur. /Çünkü daha az tablo oluşturuluyor./
#endregion

#region TPC Nasıl Uygulanır?
//Hiyerarşik düzlemde abstract olan yapılar üzerinden, OnModelCreating -> Entity fonskiyonuyla konfigürasyona girip ardından UseTpcMappingStrategy fonksiyonu
//eşliğinde kullancağımız davranışın TPC olacağını belirleyebiliriz.
#endregion

#region TPC'de Veri Ekleme
/*
await context.Technicians.AddAsync(new() { Name = "Tancan", Surname = "Fümen", Branch = "Yazılımcı", Department = "Yazılım Departmanı" });
await context.Technicians.AddAsync(new() { Name = "Can", Surname = "Sungur", Branch = "Yazılımcı", Department = "Yazılım Departmanı" });
await context.Technicians.AddAsync(new() { Name = "Hasan Fehmi", Surname = "Nemli", Branch = "Yazılımcı", Department = "Yazılım Departmanı" });

await context.SaveChangesAsync();
*/
#endregion

#region TPC'de Veri Silme
/*
//3 Id'sine sahip Teknisyeni uçurduk.
Technician? silinecek = await context.Technicians.FindAsync(3);
context.Technicians.Remove(silinecek);
*/
await context.SaveChangesAsync();

#endregion

#region TPC'de Veri Güncelleme
//2 Id'sine sahip Technician'ı Mahmutoviç olarak değiştirelim.
/*
Technician? guncellenecek = await context.Technicians.FindAsync(2);
guncellenecek.Name = "Mahmutoviç";
await context.SaveChangesAsync();
*/
#endregion

#region TPC'de Veri Sorgulama

//var datas = await context.Technicians.ToListAsync();
//Console.WriteLine();

#endregion
abstract class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
}
class Employee : Person
{
    public string? Department { get; set; }
}
class Customer : Person
{
    public string? CompanyName { get; set; }
}
class Technician : Employee
{
    public string? Branch { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Technician> Technicians { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TPT - Eski Yöntem
        //modelBuilder.Entity<Person>().ToTable("Persons");
        //modelBuilder.Entity<Employee>().ToTable("Employees");
        //modelBuilder.Entity<Customer>().ToTable("Customers");
        //modelBuilder.Entity<Technician>().ToTable("Technicians");

        //TPC - Yeni Yöntem
        modelBuilder.Entity<Person>().UseTpcMappingStrategy();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
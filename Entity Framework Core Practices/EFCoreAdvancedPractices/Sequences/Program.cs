using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Sequence Nedir?
//Veritabanında benzersiz ve ardışık sayısal değerler üreten veritabanı nesnesidir.
//Sequence herhangi bir tablonun özelliği değildir. Veritabanı nesnesidir. Birden fazla tablo tarafından kullanılabilir.
#endregion
#region Sequence Tanımlama
//Sequence'ler üzerinden değer oluştururken veritabanına özgü çalışma yapılması zorunludur.
//SQL Server'a özel yazılan Sequence tanımı örnek olarak Oracle için hata verebilir.

#region HasSequence

#endregion
#region HasDefaultValueSql
//Tanımlanan sequence'in hangi entity'lere karşılık generate edilmiş tablolarda, hangi kolonlara karşılık kullanılacağını HasDefaultValueSql fonksiyonu ile bildirebiliriz.
#endregion
#endregion

await context.Employees.AddAsync(new() { Name = "Hakan", Surname = "Taşıyan", Salary = 1000 });
await context.Employees.AddAsync(new() { Name = "Alarko", Surname = "Carrier", Salary = 1000 });
await context.Employees.AddAsync(new() { Name = "Gerçek", Surname = "Konfor", Salary = 1000 });

await context.Customers.AddAsync(new() { Name = "Tolkien" });
//Sequence tek bir nesne olarak kullanıldığı için 1 Id'si ile Tolkien, 2-3-4 ile de Employee'ler eklenecektir. Tekrar kodu çalıştırırsak
//5 Id'si ile Customer, 6-7-8 ile Employees eklenecektir. Yani her iki tablo için tek sequence kullandıldığında sadece o nesne üzerinden artış olacaktır.
await context.SaveChangesAsync();

#region Sequence Yapılandırması
//Yukarıdaki durumu özelleştirebiliriz.
#region StartsAt

#endregion
#region IncrementsBy

#endregion
#endregion
#region Sequence İle Identity Farkı
//Sequence bir veritabanı nesnesiyken, Identity ise tablolardaki bir özelliktir.
//Yani Sequence herhangi bir tabloya bağımlı değildir. İstediğimiz kadar tablo ile kullanabiliriz.
//Identity bir sonraki değeri diskten alırken Sequence ise RAM'den alır. Bu yüzden önemli ölçüde Identity'e nazaran daha hızlı, performanslı ve az maliyetlidir.
#endregion

class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }
}
class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence("EC_Sequence") //EC_Sequence isminde bir Sequence tanımladık.
            .StartsAt(100) //Sequence hangi değerden başlasın.
            .IncrementsBy(5); //Kaçar kaçar artsın. Bunları belirtmezsen 1'den başlar 1'er artar.

        //Employe içerisindeki Id kolonuna EC_Sequence ile sequence'in ürettiği değerleri ver.
        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence"); //Id kolonu için EC_Sequence'dan bir sonraki değeri getir.

        //Aynı işlemi aynı sequence'i kullanarak Customer'da da yapabiliriz. Artık artış özelliği identity'den değil EC_Sequence üzerinden olacak.
        modelBuilder.Entity<Customer>()
            .Property(c => c.Id)
            .HasDefaultValueSql("NEXT VALUE FOR EC_Sequence");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
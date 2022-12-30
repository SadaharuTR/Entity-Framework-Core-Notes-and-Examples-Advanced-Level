using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Keyless Entity Types
//Normal Entity Type'lara ek olarak primary key içermeyen querylere karşı veritabanı sorguları yürütmek için kullanılan bir özelliktir (KET).

//Genellikle aggregate operasyonların yapıldığı group by yahut pivot table gibi işlemler neticesinde elde edilen istatistiksel sonuçlar primary key
//kolonu barındırmazlar. Bizler bu tarz sorguları Keyless Entity Types özelliği ile sanki bir entity'e karşılık geliyormuş gibi çalıştırabiliriz.
#endregion

/*
Bir pivot tablo, daha geniş bir tabloyu özetleyen istatistik tablosudur. Bu özet, pivot tabloyu anlamlı bir şekilde gruplayan toplamları, 
ortalamaları veya diğer istatistikleri içerebilir. Pivot tablolar veri işlemede bir tekniktir. Yararlı bilgilere dikkat çekmek için istatistikleri 
düzenler ve yeniden düzenler. 
*/

/* SQL Sorgumuz
select p.Name, COUNT(*) [Count] FROM Persons p
JOIN Orders o
	ON p.PersonId = o.PersonId
GROUP By p.Name
*/
#region Keyless Entity Types Tanımlama
//1. Hangi sorgu olursa olsun EF Core üzerinden bu sorgunun bir entity'e karşılık geliyormuş gibi işleme/execute'a/çalıştırmaya tabi tutulabilmesi için
//o sorgunun sonucunda, bir entity'nin yine de tasarlanması gerekmektedir.

//2. Bu Entity'nin DbSet property'si olarak DbContext nesnesine eklenmesi gerekmektedir.

//3. Tanımlamış olduğumuz Entity'e OnModelCreating fonksiyonu içerisinde girip bunun bir key'i olmadığını bildirmeli (HasNoKey) ve hangi sorgunun
//çalıştırılacağı da ToView vs. gibi işlemlerle ifade edilmelidir.
/*
var datas = await context.PersonOrderCounts.ToListAsync();
Console.WriteLine();
*/
#region Keyless Attribute'u
#endregion
#region HasNoKey Fluent API'ı
#endregion
#endregion

#region Keyless Entity Types Özellikleri Nelerdir?
//Primary Key kolonu OLMAZ! OLAMAZ!! OLAMAAAZ!!!
//Change Tracker mekanizması aktif olmayacaktır. Bu özellik neticesinde elde etmiş olduğumuz veriler, üzerindeki delete, insert, update geçerli olmayacaktır.
//TPH olarak Entity hiyerarşisinde kullanılabilir lakin diğer kalıtımsal ilişkilerde kullanılamaz!
#endregion

//[Keyless] //istersek bu entity'nin PK'si olmayacağını Keyless atribute ile bildirebiliriz. Bunu yaparsak artık FluenAPI'da HasNoKey'i kullanmak zorunda değiliz.
public class PersonOrderCount //bu sınıf ve property'leri gelecek olan verileri karşılayacak bir entity oluşturduk.
{
    public string Name { get; set; }
    public int Count { get; set; }
}
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
    public DbSet<PersonOrderCount> PersonOrderCounts { get; set; } //veri karşılamak için oluşturmuş olduğumuz Entity'i DbSet olarak context nesnesine tekleyelim.
    //DbSet property'sinin bir tablo mu bir view'mi bir SP'mi karşılığı olduğunu bildirmemiz lazım. Yukarıdkai gibi SQL sorguları için view daha uygun.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Person>()
            .HasMany(p => p.Orders)
            .WithOne(o => o.Person)
            .HasForeignKey(o => o.PersonId);

        //entity'nin view'e karşılık geldiğini ve PK'si olmayacağını bildiriyoruz.
        modelBuilder.Entity<PersonOrderCount>()
            .HasNoKey()
            .ToView("vw_PersonOrderCount");
        //eğer ki bu bildirimi yapmasaydık yeni bir mig_3 basıldığında bu bir tablo olarak algılanacaktı ve migration'daki create yapılanması inşa edilecekti.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");
    }
}
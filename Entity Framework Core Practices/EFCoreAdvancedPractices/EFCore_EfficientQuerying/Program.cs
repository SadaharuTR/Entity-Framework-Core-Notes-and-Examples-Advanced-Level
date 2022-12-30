
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region EF Core Select Sorgularını Güçlendirme Teknikleri

#region IQueryable - IEnumerable Farkı

//IQueryable, bu arayüz üzerinde yapılan işlemler direkt generate edilecek olan sorguya yansıtılacaktır.
//IEnumerable, bu arayüz üzerinde yapılan işlemler temel sorgu neticesinde gelen ve in-memorye yüklenen instance'lar üzerinde gerçekleştirilir.
//Yani sorguya yansıtılmaz.

//IQueryable ile yapılan sorgulama çalışmalarında SQL sorguyu hedef verileri elde edecek şekilde generate edilecekken, IEnumerable ile yapılan
//sorgulama çalışmalarında SQL daha geniş verileri getirebilecek şekilde execute edilerek hedef veriler in-memory'de ayıklanır.

//IQueryable hedef verileri getirirken, IEnumerable hedef verilerden daha fazlasını getirip in-memory'de ayıklar.

//Her ikisi de Deferred Execution-Ertelenmiş Çalışma davranışı sergilerler. Tetiklendikleri noktada çalışırlar. 
//Yani her iki arayüz üzerinden de oluşturulan islemi execute edebilmek için ToList gibi tetikleyici fonksiyonları ya da
//foreach gibi tetikleyici işlemleri gerçekleştirmemiz gerekmektedir.

#region IQueryable
/*
var persons = await context.Persons.Where(p => p.Name.Contains("A"))
                             .Take(3)
                             .ToListAsync();

foreach (var item in persons)
{   }

var persons = await context.Persons.Where(p => p.Name.Contains("A"))
                             .Where(p => p.PersonId > 3)
                             .Take(3)
                             .Skip(3)
                             .ToListAsync();
*/
#endregion

#region IEnumerable
/*
var persons = context.Persons.Where(p => p.Name.Contains("a")) //burada hala IQueryable'dayız.
                             .AsEnumerable() //ile IEnumerable'a geçiş yap. 'dan sonraki tüm şartları In Memory'de yap.
                             .Take(3)
                             .ToList(); //Async olarak kullanamayız.
*/
#endregion

#region AsQueryable
//IEnumerable üzerinden yapılan bir çalışmayı IQueryable'a dönüştürmek için kullanılan fonksiyondur.
#endregion
#region AsEnumerable
//IQueryable üzerinden yapılan bir çalışmayı IEnumerable'a dönüştürmek için kullanılan fonksiyondur.

#endregion
#endregion

//SELECT SORGULARINI DÖNÜŞTÜRMEK

#region Yalnızca İhtiyaç Olan Kolonları Listeleyin - Select
/*
var persons = await context.Persons.Select(p => new
{
    p.Name
    //p.PersonId //ihtiyaç varsa.
}).ToListAsync();
*/
#endregion

#region Result'ı Limitleyin - Take
//await context.Persons.Take(50).ToListAsync(); //Take yani TOP ile sınırlayın.
#endregion

#region Join Sorgularında Eager Loading Sürecinde Verileri Filtreleyin
/*
var persons = await context.Persons.Include(p => p.Orders
                                                  .Where(o => o.OrderId % 2 == 0)
                                                  .OrderByDescending(o => o.OrderId)
                                                  .Take(4))
    .ToListAsync();

//verimsiz yöntem;
foreach (var person in persons)
{
    var orders = person.Orders.Where(o => o.CreatedDate.Year == 2022);
}
*/
#endregion

#region Şartlara Bağlı Join Yapılacaksa Eğer Explicit Loading Kullanın
/*
//var person = await context.Persons.Include(p => p.Orders).FirstOrDefaultAsync(p => p.PersonId == 1); //maliyetli.

var person = await context.Persons.FirstOrDefaultAsync(p => p.PersonId == 1);

if (person.Name == "Ayşe") //şart sağlanıyorsa Join işlemini yap.
{
    //Order'larını getir...
    await context.Entry(person).Collection(p => p.Orders).LoadAsync(); //bunu da Explicit Loading ile yap.
}
//ilgili veriye uygun karşılıklı ilişkisel verileri çekmiş olduk.
*/
#endregion

#region Lazy Loading Kullanırken Dikkatli Olun!

#region Riskli Durum
//Ne kadar Person varsa her bir Person'a karşılık Order sorgusu gönderir. Her bir döngüde ilgili Person'a karşılık Order'ları Lazy Loading yüklemektedir.
/*
var persons = await context.Persons.ToListAsync();

foreach (var person in persons)
{
    foreach (var order in person.Orders)
    {
        Console.WriteLine($"{person.Name} - {order.OrderId}");
    }
    Console.WriteLine("***********");
}
*/
#endregion

#region İdeal Durum
/*
//1-Select ile her bir personel'in adını ve order bilgisini çekelim.
var persons = await context.Persons.Select(p => new { p.Name, p.Orders }).ToListAsync(); //3-Çünkü zaten order bilgilerini burada çektik.

foreach (var person in persons)
{   //2-bu işlemden sonra person'ların Order'larına geldiğimizde her döngü tetiklendiğinde tekrardan Order sorgularının gitmediğini görürüz.
    foreach (var order in person.Orders)
    {
        Console.WriteLine($"{person.Name} - {order.OrderId}");
    }
    Console.WriteLine("***********");
}
*/
#endregion
#endregion

#region İhtiyaç Noktalarında Ham SQL Kullanın - FromSql
//Ayıp değil kullan. Ama ihtiyaç noktasında.
#endregion

#region Asenkron Fonksiyonları Tercih Edin

#endregion

#endregion

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Order> Orders { get; set; }
}
public class Order
{
    public int OrderId { get; set; }
    public int PersonId { get; set; }
    public string Description { get; set; }

    public virtual Person Person { get; set; }
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
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True")
            .UseLazyLoadingProxies();
    }
}
using Microsoft.EntityFrameworkCore;
using System.Reflection;

ApplicationDbContext context = new();

#region Global Query Filters Nedir?
//Bir entity'e özel uygulama seviyesinde genel/ön kabullü şartlar oluşturmamızı ve böylece verileri global bir şekilde filtrelememeizi sağlayan bir özelliktir.
//Böylece belirtilen entity üzerinden yapılan tüm sorgulamalarda ekstradan bir şart ifadesine gerek kalmaksızın filtreleri otomatik uygulayarak hızlıca sorgulama
//yapmamızı sağlamaktadır.

//Genellikle uygulama bazında aktif(IsActive) gibi verilerle çalışıldığı durumlarda,
//MultiTenancy uygulamalarda TenantId tanımlarken vs. kullanılabilir.
//Bkz. https://www.gencayyildiz.com/blog/asp-net-core-multitenancy-uygulama-nasil-olusturulur/
#endregion

#region Global Query Filters Nasıl Uygulanır?

//Bu uygulama üzerinde yapılan tüm Person sorgulamalarında default olarak aktif Person'lar üzerinden sorgulama yapmak istersek,

//Her sorguda tek tek ekleyebiliriz ya da ön tanımlı hale getirebiliriz.
//await context.Persons.Where(p => p.IsActive).ToListAsync(); //buradaki Where sorgusunu oto-uygulatabiliriz.

//HasQueryFilter()'daki işlemlerden sonra aşağıdaki sorgularda Where(p => p.IsActive) şartı otomatik sağlanacaktır.
//await context.Persons.ToListAsync();
//await context.Persons.FirstOrDefaultAsync(p => p.Name.Contains("a") || p.PersonId == 3);

#endregion

#region Navigation Property Üzerinde Global Query Filters Kullanımı
//Uygulama çağında kullanacağımız person nesnelerinin hepsi en az 1 satış yapmış olmalı, hiç satış yapmayanlar sorgulama neticesinde gelmesin.
/*
await context.Persons
    .Include(p => p.Orders)
    .Where(p => p.Orders.Count() > 0)
    .ToListAsync();
*/
//Her ihtiyaç noktasında bu şartı yazabiliriz.
//Ya da NP üzerinden GQF kullanabiliriz.

//HasQueryFilter()'daki işlemlerden sonra modelBuilder.Entity<Person>().HasQueryFilter(p => p.Orders.Count > 0);
//artık biz direkt Person'u sorgulasak dahi arkaplanda bu Person'un ilişkili olduğu Order'larda en az 1 tane kaydının olması gerektiğini EF Core anlayıp ona göre
//sorgusunu generate edecektir.
/*
var p1 = await context.Persons
    .AsNoTracking()
    .Include(p => p.Orders)
    .Where(p => p.Orders.Count > 0)
    .ToListAsync();

//iki sorgu da aynı sonucu verecektir. Üstteki sorguyu HasQueryFilter(p => p.Orders.Count > 0) işlemini önceden belirttiğimiz için aşağıdaki gibi sadeleştirebildik.

var p2 = await context.Persons.AsNoTracking().ToListAsync();
*/
//Gelen sorgulardaki veriler Change Tracker'dan dolayı birbirlerini etkileyebilirler. Bundan dolayı .AsNoTracking() ile Change Tracker'ı pasif hale getirelim.
//Bu şekilde ilişkisel tablolardaki şartlarımızı global bir şekilde sorguya ekleyip uzun uzun kodlar yazmaksızın sade query'ler inşa edebiliriz.
//Console.WriteLine();
#endregion
#region Global Query Filters Nasıl Ignore Edilir? - IgnoreQueryFilters
//Varsa bir Global Query Fiters'ı o anki sorgu için iptal etmek için IgnoreQueryFilters()'ı kullanırız.
//var person1 = await context.Persons.ToListAsync(); //şart devrede
//var person2 = await context.Persons.IgnoreQueryFilters().ToListAsync(); //şart uçtu.

//Console.WriteLine();
#endregion
#region Dikkat Edilmesi Gereken Husus!
//Global Query Filter uygulanmış bir kolona farkında olmaksızın şart uygulanabilmektedir. Bu duruma dikkat edilmelidir.

//await context.Persons.Where(p => p.IsActive).ToListAsync(); //iki kere aynı uygulamış olduk. Gereksiz şart maliyeti.

#endregion

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public List<Order> Orders { get; set; }
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

        //Global Query Filters Nasıl Uygulanır? başlığı.
        //modelBuilder.Entity<Person>().HasQueryFilter(p => p.IsActive); //Person entity'sine karşılık verilecek ön şartı (p => p.IsActive) sorguya ekle.

        //NP ile..
        modelBuilder.Entity<Person>().HasQueryFilter(p => p.Orders.Count > 0);
    }

    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");
    }
}
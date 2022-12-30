using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();
#region Explicit Loading

//Oluşturulan sorguya eklenecek verilerin şartlara bağlı bir şekilde/ihtiyaçlara istinaden yüklenmesini sağlayan bir yaklaşımdır.

//Ameleus Yöntemi
//employee'lardan Id'si 2 olanı elde edelim.
//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2); 
//buradaki sorguya Include ile Order'ları eklersek şartın sağlanmadığı durumlarda da Order'lar yine elde edilmiş olacak.
//fakat if bloğu içinde şartın geçerli olduğu durumlarda Order'ları elde etmek istiyorsak aşağıdaki gibi yapabiliriz.
//if (employee.Name == "Rıfkı") //bu employee'nin adı Rıfkı ise Order'ları elde et.
//{
//    var orders = await context.Orders.Where(o => o.EmployeeId == employee.Id).ToListAsync(); //Şarta bağlı Order'ları getirmiş olduk.
//}

//Fakat Explicit Loading bize daha kısa çözümler sunuyor.

#region Reference

//Explicit Loading sürecinde ilişkisel olarak sorguya eklenmek istenen tablonun navigation property'si eğer ki "tekil" bir türse
//bu tabloyu reference ile sorguya ekleyebilmekteyiz.

var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
////...
////...işlemler işlemler
////...
//işlemler sonucu gün geldi bu Employee'un Region'una ihtiyacımız oldu. Bu durumda referance'ı kullanabiliriz.
await context.Entry(employee).Reference(e => e.Region).LoadAsync();
//bunun için önce context üzerinden Enrty'e gidiyoruz. Bu Entry'e işlem yapılacak objeyi-employee veriyoruz. Verdikten sonra Referance fonksiyonunu kullanarak
//bu objenin içerisinde hangi nav. property'i tabloya ekleyeceksek onu belirtiyoruz. LoadAsync ile de işlemi tamamlıyoruz.

//Bu işlemler sonucunda employee nesnesine Region'la ilgili ilişkisel verileri de eklemiş oluyoruz.

//Console.WriteLine();
#endregion
#region Collection

//Explicit Loading sürecinde ilişkisel olarak sorguya eklenmek istenen tablonun navigation property'si eğer ki çoğul/koleksiyonel bir türse
//bu tabloyu Collection ile sorguya ekleyebilmekteyiz.

//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
//...
//...bişiler bişiler
//...
//await context.Entry(employee).Collection(e => e.Orders).LoadAsync();

//Console.WriteLine();
#endregion

#region Collection'lar da Aggregate Operatör Uygulamak
//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
//...
//...
//...
//burada gelen verilerin üzerinde direkt işlem de yapılabilir.
//Collection üzerinden eklemiş olduğumuz koleksiyonel değeri, sorgu sonucunu elde edip, üzerinde Aggregate işlemleri uygulayabiliriz.
//var count = await context.Entry(employee).Collection(e => e.Orders).Query().CountAsync();
//Query ile IQeryable'a dönüştürüp Count Aggregate fonksiyonunu uyguladık. Artık bu employee'nin kaç tane Orders'ı varmış öğrenebiliriz.
//Sum, average gibi işlemleri de yapabiliriz.

Console.WriteLine();
#endregion
#region Collection'lar da Filtreleme Gerçekleştirmek

//var employee = await context.Employees.FirstOrDefaultAsync(e => e.Id == 2);
////...
////...cart curt
////...
//bugün yapılmış olan siparişleri elde et diyerek filtreleme uygulayabiliriz.
//var orders = await context.Entry(employee).Collection(e => e.Orders).Query().Where(q => q.OrderDate.Day == DateTime.Now.Day).ToListAsync();

//önce employee'u çekip bu employee'da şarta uygun olan Order'ları veritabanından çekip, bunları ilişkilendirip sorguladık.
#endregion
#endregion

public class Employee
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }

    public List<Order> Orders { get; set; }
    public Region Region { get; set; }
}
public class Region
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees { get; set; }
}
public class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }

    public Employee Employee { get; set; }
}


class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Region> Regions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
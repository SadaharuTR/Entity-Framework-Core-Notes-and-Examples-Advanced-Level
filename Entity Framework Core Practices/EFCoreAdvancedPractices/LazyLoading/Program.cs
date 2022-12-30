
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

ApplicationDbContext context = new();

#region Lazy Loading Nedir?
//Navigation property'ler üzerinde bir işlem yapılmaya çalışıldığı taktirde ilgili property'nin/ye temsil ettiği/karşılık gelen tabloya özel bir sorgu oluşturulup +
//execute edilmesini ve verilerin yüklenmesini sağlayan bir yaklaşımdır.
#endregion

//var employee = await context.Employees.FindAsync(2);
//Console.WriteLine(employee.Region.Name); //null hatası verir.

//Eğer ki Region üzerinde Lazy Loading işlemini uygularsak ne zamanki employee'dan Region'u çağırırız, o zaman daha Name'i çağırılmadan veritabanına gidilir ve gerekli
//SQL oluşturulur ondan sonra execute edildikten sonra veriler getirilir, son olarak Name'i yazdırmış oluruz.
//Yani Lazy Loading biz ilgili NP'i kullanmak istediğimiz anda o NP'ye karşılık tabloyu veritabanında sorgulayıp bize getirir.

//OnConfiguring'de Lazy Loading'i aktif ettikten sonra açlıştırırsak bu sefer de NP'lerin virtual olması gerektiği hatasını verir. Eğer ki proxy üzerinden Lazy Loading
//gerçekleştiriyorsak NP'lerimizin virtual ile işaretlenmiş olması gerekmektedir. (ilgili başlıkta tekrar belirtelim.)

#region Proxy'lerle Lazy Loading
//Genellikle Proxy yapılanması kullanılır. Aşağıdaki kütüphaneki yapılanmalar kullanılır. NuGet üzerinden yükleyelim.
//Microsoft.EntityFrameworkCore.Proxies
//Kullanmak için OnConfiguring'de çalışmamızı yaparız.

#region Property'lerin Virtual Olması
//Eğer ki proxy'ler üzerinden lazy loading operasyonu gerçekleştiriyorsanız Navigtation Propertylerin virtual ile işaretlenmiş olması gerekmektedir.
//Aksi taktirde patlama meydana gelecektir.

//Gerekli ayarlamalardan sonra aşağıdaki kod çalışıp ekrana Çemişgezek yazacaktır. 
//var employee = await context.Employees.FindAsync(2);
//Console.WriteLine(employee.Region.Name);

#endregion
#endregion

#region Proxy Olmaksızın Lazy Loading
//Proxy'ler tüm platformlarda desteklenmeyebilir. Böyle bir durumda manuel bir şekilde lazy loading'i uygulamak mecburiyetinde kalabiliriz.

//Manuel yapılan Lazy Loading operasyonlarında Navigation Proeprtylerin virtual ile işaretlenmesine gerek yoktur!

#region ILazyLoader Interface'i İle Lazy Loading
//Microsoft.EntityFrameworkCore.Abstractions
var employee = await context.Employees.FindAsync(2);
#endregion
#region Delegate ile Lazy Loading
//var employee = await context.Employees.FindAsync(2);
#endregion
#endregion

#region N+1 Problemi
//ilkin region'lara ait bir sorgu, sonra employee'lara ait bir sorgu, üçüncü olarak ise her bir order'la ilgili employee'a karşılık döngü döndükçe bir sorgu oluşturulmuştur.
//4 tane employee varsa 4 tane Order sorgusu oluşturulduğunu görürüz.
//İşte bu Lazy Loading'in maliyetli oluşunu gösterir. Buna N+1 problemi denir. Çözümü de Lazy Loading kullanmaktan kaçınmak.
/*
var region = await context.Regions.FindAsync(1);
foreach (var employee in region.Employees)
{
    var orders = employee.Orders;
    foreach (var order in orders)
    {
        Console.WriteLine(order.OrderDate);
    }
}
*/
#endregion

//Lazy Loading, kullanım açısından oldukça maliyetli ve performans düşürücü bir etkiye sahiptir.
//O yüzden kullanırken mümkün mertebe dikkatli olmalı ve özellikle navigation property'lerin döngüsel tetiklenme durumlarında lazy loading'i tercih etmemeye odaklanmalıyız.
//Aksi taktirde her bir tetiklemeye karşılık aynı sorguları üretip execute edecektir. Bu durumu N+1 Problemi olarak nitelendirmekteyiz.
//Mümkün mertebe, ilişkisel verileri eklerken Lazy Loading kullanmamaya özen göstermeliyiz.

Console.WriteLine();

#region Proxy İle Lazy Loading

public class Employee
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }
    public virtual List<Order> Orders { get; set; }
    public virtual Region Region { get; set; }
}
public class Region
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}
public class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }
    public virtual Employee Employee { get; set; }
}

#endregion
#region ILazyLoader Interface'i ile Lazy Loading
/*
public class Employee
{
    //manuel olarak LazyLoading'i gerçekleştirebilmemiz için ILazyLoader türünden parametre alan bir constructor daha oluşturmamız gerekiyor.
    //bunu manuel bir şekilde LazyLoading oluşturacağımız tüm Entity'lerde gerçekleştirmemiz lazım.
    //bu oluşturduğumuz parametreyi bir referansa atayalım ve bu referans üzeridnen manuel lazy loading operasyonlarını gerçekleştirelim.
    ILazyLoader _lazyLoader;
    //Region'a karşılık bir Lazy Loading yapacağımız için Region'la ilgili bir referans oluşturmamız lazım.
    Region _region;
    public Employee() { }
    public Employee(ILazyLoader lazyLoader)
        => _lazyLoader = lazyLoader;
    public int Id { get; set; }
    public int RegionId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }

    //NP'leri virtual ile işaretleyelim. (Proxy işlemleri için!) (Manuel Lazy Loading'de buna gerek yok.)
    public List<Order> Orders { get; set; }
    public Region Region
    {
        //get ile lazy loader üzerinden Load operasyonunu gerçekleştir.
        get => _lazyLoader.Load(this, ref _region); //"bu" entity üzerinde _region referans'sına karşılık yap. Referansı ref ile işaretlemek lazım.
        //Bu operasyonu yaptıktan sonra her Region NP tetiklendiğinde Lazy Loading'i devreye sokmuş olacağız.
        set => _region = value;
    } //seti ile Region referansına verilen değeri/value'u atamış-işaretlemiş oluyoruz.

}
public class Region
{
    ILazyLoader _lazyLoader;
    ICollection<Employee> _employees;
    public Region() { }
    public Region(ILazyLoader lazyLoader)
        => _lazyLoader = lazyLoader;
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees
    {
        get => _lazyLoader.Load(this, ref _employees);
        set => _employees = value;
    }

}
public class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }
    public Employee Employee { get; set; }
}
*/
#endregion

#region Delegate İle Lazy Loading
/*
public class Employee
{
    Action<object, string> _lazyLoader;
    Region _region;
    public Employee() { }
    //buradaki object bizim LL işlemine tabi tutacağımız Entity'nin türünü, string ise ismini bildirir.
    public Employee(Action<object, string> lazyLoader)
        => _lazyLoader = lazyLoader;
    public int Id { get; set; }
    public int RegionId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Salary { get; set; }
    public List<Order> Orders { get; set; }
    public Region Region
    {
        get => _lazyLoader.Load(this, ref _region); //burada _lazyLoader üzerinden Load fonksiyonunu devreye sokarak ilgili entity'e karşılık yani o anki
        //NP'e karşılık olan neyse onları LL'de elde etmiş oluyoruz.
        set => _region = value;
    }
}
public class Region
{
    Action<object, string> _lazyLoader;
    ICollection<Employee> _employees;
    public Region() { }
    public Region(Action<object, string> lazyLoader)
        => _lazyLoader = lazyLoader;
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Employee> Employees
    {
        get => _lazyLoader.Load(this, ref _employees);
        set => _employees = value;
    }
}
public class Order
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }
    public Employee Employee { get; set; }
}

//Action delegate'leri üzerinden yapılan Lazy Loading çalışmalarında Load fonksiyonu referanslar için kullanılabilir değil. Extension ile bu durum çözülür.
static class LazyLoadingExtension
{
    //Not: Metodu tanımlarken referans yolu ile iletilmek istenen değişkenin önüne "ref" yazılmalıdır.

    //Action<object, string> türüne aşağıdaki metodu extension olarak ekliyoruz. Lazım olan navigation'un bizzat kendisini almak için türünü bilmemiz lazım.
    //türünü bilebilmek için önce ref yazıp ardından o an türünü bilemeyeceğimiz için Load<TRelated> ile generic bir yapılanma olarak ortaya koyuyoruz. Parametrelerde de
    //ref TRelated navigation türünden bir navigation ile çalışacağımızı bildiriyoruz. Son olarak hangi NP'de çalışacaksak onun bize ismini getirecek bir parametre tanımlayıp
    //ismini verdirtmemek için CallerMemberName attirbute'unu kullanırız. Bu attribute navigationName parametresine değerini hangi member'dan çağırılıyorsa onun adını vererek
    //otomatik/dinamik bir şekilde halledecektir. Aşağıdaki fonksiyonu nerede tetikleyeceksek, oraya o tetiklenen yapının adını navigationName parametresine
    //otomatik verecektir. Kullanabilmek için de = null ile default bir değer vermek lazım.
    public static TRelated Load<TRelated>(this Action<object, string> loader, object entity, ref TRelated navigation, [CallerMemberName] string navigationName = null)
    {
        loader.Invoke(entity, navigationName); //loader bir delegate olduğu için önce Invoke ile tetikle. İlk parametreye entity'i, ikincisine de bu entity içerisinde
        //lazy loading'e tabii tutulan NP'i ver.
        //Load fonksiyonunda Action'umuzu burada tetiklemiş olduk.
        return navigation;
        //bundan sonra LL'e tabii tutulmuş navigation hangisiyse onu return et.
    }
}
*/
#endregion

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
        //Proxy ile Lazy Loading;
        //optionsBuilder.UseLazyLoadingProxies();
        //optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
        //ya da aşağıdaki gibi,
        optionsBuilder
            .UseLazyLoadingProxies(true) //Interface-Delagate ile Lazy Loading işlemi için burayı false yaptık. Proxy için true olacak.
            .UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");       
        //bu işlemi yaptıktan sonra uygulamada Lazy Loading yaklaşımı Enabled hale gelmiş olacak.
    }
}
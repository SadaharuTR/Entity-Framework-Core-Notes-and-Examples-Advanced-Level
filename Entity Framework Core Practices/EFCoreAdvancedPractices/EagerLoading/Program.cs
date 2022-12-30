using Loading_Related_Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Loading Related Data

#region Eager Loading
//Eager loading, generate edilen bir sorguya ilişkisel verilerin parça parça eklenmesini sağlayan ve bunu yaparken iradeli/istekli bir şekilde yapmamızı sağlayan bir yöntemdir.
//Eager Loading arkaplanda üretilen sorguya Join uygular. Include kullandığımızda SQL tarafında uygun Join işlemi gerçekleştirilir. Sorguyla hedef tablo birleştirilir.

#region Include
//Eager loading operasyonunu yapmamızı sağlayan bir fonksiyondur.
//Yani üretilen bir sorguya diğer ilişkisel tabloların dahil edilmesini sağlayan bir işleve sahiptir..

//var employees = await context.Employees.Include("Orders").ToListAsync();
//var employees = await context.Employees.Include(e => e.Orders).ToListAsync(); /aynısı. ama tip güvenli.

//var employees = await context.Employees //Include fonksiyonu kullanarak istediğimiz kadar tabloyu o anki sorguya dahil edebiliyoruz.
//    .Include(e => e.Region)
//    .Where(e => e.Orders.Count > 2) //araya şartlar ekleyebiliriz. Şartı Include'lardan önceye de alabiliriz.
//    .Include(e => e.Orders)
//    .ToListAsync();

#endregion

#region ThenInclude
//ThenInclude, üretilen sorguda Include edilen tabloların ilişkili olduğu diğer tabloları da sorguya ekleyebilmek için kullanılan bir fonksiyondur. 
//Eğer ki, üretilen sorguya Include edilen navigation property koleksionel bir property ise işte o zaman bu property üzerinden diğer ilişkisel tabloya
//erişim gösterilememektedir. Böyle bir durumda koleksiyonel propertylerin türlerine erişip, o tür ile ilişkili diğer tabloları da sorguya eklememizi sağlayan fonksiyondur.

//İlişkisel tablolar arasında derece farketmeksizin, navigation property tekil bir türse propertlere erişerek member'lar üzerinden çalışabiliriz.
//var orders = await context.Orders
//    //.Include(o => o.Employee) bu satırı kullanma zorunluluğu önceki sürümde kalktı. Sadece alttaki satırla hem Employee hem Region'u getirebiliriz.
//    .Include(o => o.Employee.Region)
//    .ToListAsync();

//Ama bir koleksiyonel navigation property'den bu türün ilişkili olduğu başka bir tabloya gitmek gerekiyorsa ThenInclude kullanılır.
//var regions = await context.Regions
//    .Include(r => r.Employees) //Include ederek buradaki sorguya dahil edilmiş olan koleksiyonal Employees NP'nin türünü ThenInclude ile elde edip,
//    .ThenInclude(e => e.Orders) //ve bu türün içerisindeki Orders property'sini de bu sorguya eklliyoruz.
//    .ToListAsync(); //işte bunu da ThenInclude ile yapıyoruz.

#endregion

#region Filtered Include
//Sorgulama süreçlerinde Include yaparken sonuçlar üzerinde filtreleme ve sıralama gerçekleştirebilmemizi sağlayan bir özleliktir.

//var regions = await context.Regions //regionlardan yola çıkıp tüm Employees'ları sorguya dahil edelim.
//    .Include(r => r.Employees.Where(e => e.Name.Contains("a")).OrderByDescending(e => e.Surname)) 
//where şartı ile adında sadece a harfi geçenleri dahil et ve soyadına göre sırala şartı ekleyerek filtrele.
//    .ToListAsync();

//Desteklenen fonksiyonlar : Where, OrderBy, OrderByDescending, ThenBy, ThenByDescending, Skip, Take

//Change Tracker'ın aktif olduğu durumlarda Include edilmiş sorgular üzerindeki filtreleme sonuçları beklenmeyen olabilir.
//Bu durum, daha önce sorgulanmış ve Change Tracker tarafından takip edilmiş veriler arasında filtrenin gereksinimi dışında kalan veriler için söz konusu olacaktır.
//Bundan dolayı sağlıklı bir filtered include operasyonu için change tracker'ın kullanılmadığı sorguları tercih etmeyi düşünebilirsiniz.

#endregion

#region Eager Loading İçin Kritik Bir Bilgi
//EF Core, önceden üretilmiş ve execute edilerek verileri belleğe alınmış olan sorguların verilerini, sonraki sorgularda KULLANIR!
//Yani herhangi bir sorgulama neticesinde herhangi bir tablonun verilerini belleğe aldıysak eğer, sonraki sorgu süreçlerinde bu veriler kullanılacaktır.

//Orders'taki verileri sorgulayıp, ToList ile execute edip belleğe aldık.
//var orders = await context.Orders.ToListAsync();

//benzer mantıkta Employees'i sorgulayalım ve execute edip belleğe alalım.
//var employees = await context.Employees.ToListAsync();
//bu sorguda Orders ile ilgili bir Include yapmamamıza rağmen Orders bilgileri de gelecektir.
//EF Core Order'ları her bir Employee'a karşılık ilişkili bir biçimde direkt verir.

//Gerçek hayat işlemlerinde önceden bir veriyi sorgulayıp belleğe aldığımızdan eminsek daha sonraki operasyonlarımızda bir daha o tabloyu Include etmemize
//gerek kalmadığını bilmemiz gerekir. Masraftan düş...
//var employees = await context.Employees.Include(e => e.Orders).ToListAsync(); hiç gerek yok.

#endregion

#region AutoInclude - EF Core 6
//Uygulama seviyesinde bir Entity'e karşılık yapılan tüm sorgulamalarda "kesinlikle" bir tabloya Include işlemi gerçekleştirilecekse eğer bunu her bir sorgu için
//tek tek yapmaktansa merkezi bir hale getirmemizi sağlayan özelliktir. OnModelCreating'de gerekli işlemler yapılır.

//gerekli işlemleri yapıp aşağıdaki sorguyu çalıştırdığımızda Region ile Join'lenip gelecektir.
//var employees = await context.Employees.ToListAsync();
#endregion

#region IgnoreAutoIncludes
//AutoInclude konfigürasyonunu sorgu seviyesinde pasifize edebilmek için kullandığımız fonksiyondur.

//var employees = await context.Employees.IgnoreAutoIncludes().ToListAsync();
//bu sorgu için sadece Employees sorgulanacaktır.
#endregion

#region Birbirlerinden Türetilmiş Entity'ler Arasında Include Operasyonu (Kalıtımda Include)

#region Cast Operatörü İle Include
//normalde referans p, person olduğu için Order'ları göremiyoruz, fakat burada, person'u Employee'a cast et ardından içerisindeki order'ları sorguya dahil edersek görürüz.
var persons1 = await context.Persons.Include(p => ((Employee)p).Orders).ToListAsync();
#endregion
#region as Operatörü İle Include
var persons2 = await context.Persons.Include(p => (p as Employee).Orders).ToListAsync();
#endregion
#region 2. Overload İle Include
var persons3 = await context.Persons.Include("Orders").ToListAsync();
#endregion
#endregion
Console.WriteLine();
#endregion
#endregion



public class Person
{
    public int Id { get; set; }

}
public class Employee : Person
{
    //public int Id { get; set; }
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
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Region> Regions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //hangi entity'e karşılık AutoInclude yapılacaksa-Employee girilir.
        modelBuilder.Entity<Employee>()
            .Navigation(e => e.Region) //Navigation metodu üzerinden ilgili Navigation Property bildirilir.
            .AutoInclude();
        //her Employee sorguladığımda kesinlikle Region bekliyorsam bunu AutoInclude ile her seferinde yükle demiş oluruz.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
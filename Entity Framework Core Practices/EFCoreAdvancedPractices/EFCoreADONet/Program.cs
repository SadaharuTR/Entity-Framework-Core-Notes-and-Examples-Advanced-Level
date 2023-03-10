
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Transactions;

ApplicationDbContext context = new();

#region Database Property'si
//Database property'si veritabanını temsil eden ve EF Core'un bazı işlevlerinin detaylarına erişmemizi sağlayan bir property'dir.
#endregion

/*
SQL'de Transaction
Bir tabloda işlem yapıyor olun. Bu işlem ister insert, update, delete işlemi olsun. Bu noktada eğer yapacağınız işlem farklı bir veya bir kaç noktayı etkileyecek 
ise ve bu noktaların bir tanesinde dahi hata almanız durumunda yapılan tüm işlemleri (o esnada) geri almak istiyorsanız kesinlikle ihtiyaç duyacağınız bir metoddur 
Transaction.
*/

#region BeginTransaction
//EF Core, transaction yönetimini otomatik bir şekilde kendisi gerçekleştirmektedir. Eğer ki transaction yönetimini manuel olarak anlık ele almak istiyorsak
//BeginTransaction fonksiyonunu kullanabiliriz.

//IDbContextTransaction transaction = context.Database.BeginTransaction();
//Artık transaction işlemlerini transaction ile manuel olarak anlık ele aldık.

#endregion

#region CommitTransaction
//EF Core üzerinde yapılan çalışmaların commit edilebilmesi için kullanılan bir fonksiyondur.
//context.Database.CommitTransaction(); 
//transaction'u manuel bir şekilde ele almaksızın commit işlemini devreye sokmak istiyorsak direkt Database üzerinden bu fonksiyonu kullanabiliriz.

#endregion

#region RollbackTransaction
//EF Core üzerinde yapılan çalışmaların rollback edilebilmesi için kullanılan bir fonksiyondur.
//context.Database.RollbackTransaction();

#endregion

#region CanConnect
//Verilen connection string'e karşılık bağlantı kurulabilir bir veritabanı var mı yok mu bunun bilgisini bool türde veren bir fonksiyondur.
/*
bool connect = context.Database.CanConnect();
Console.WriteLine(connect);
*/
#endregion

#region EnsureCreated
//EF Core'da tasarlanan veritabanını migration kullanmaksızın, runtime'da yani kod üzerinde veritabanı sunucusuna inşa edebilmek için kullanılan bir fonksiyondur.
//context.Database.EnsureCreated();
//DbContext context nesnemizin içerisindeki veritabanı tasarımı her neyse ona uygun bir veritabanını arka planda create edecektir. Veritabanında oluşturacaktır.
//Geriye bool değer döndürür. Başarılı olup olmadığını oradan anlayabiliriz.
#endregion

#region EnsureDeleted
//İnşa edilmiş veritabanını runtime'da silmemizi sağlayan bir fonksiyondur.
//context.Database.EnsureDeleted();
#endregion

#region GenerateCreateScript
//Context nesnesinde yapılmış olan veritabanı tasarımı her ne ise ona uygun bir SQL Script'ini string olarak veren metottur.
/*
var script = context.Database.GenerateCreateScript();
Console.WriteLine(script);
*/
//Veritabanını tasarlayabilmek için EF Core üzerinde ne yaptıysak (insert, update, constraint...) özetini SQL olarak verir.

#endregion
#region ExecuteSql
//Veritabanına yapılacak Insert, Update ve Delete sorgularını yazdığımız bir metottur. Bu metot işlevsel olarak alacağı parametreleri SQL Injection saldırılarına
//karşı korumaktadır. 

//var result = context.Database.ExecuteSql($"INSERT Persons VALUES('Nalan')"); // ile yeni veri ekleyebiliriz.
/*
string name = Console.ReadLine(); //dışarıdan redlina ile aldığımız name değerini,
var result = context.Database.ExecuteSql($"INSERT Persons VALUES('{name}')"); 

//string interpolation kullanarak basıyoruz. Burada kullanmaış olduğumuz yapılanma,
//ExecuteSql'de arkaplanda SQL Parameter'a dönüştürülüyor ve o şekilde sorguya işleniyor. Bu sayede SQL Injection açıkları da mimari tarafından kapatılmış oluyor.
*/
/*Dipnot
SQL injection, web uygulamasının yaptığı SQL sorgusuna müdahale edilerek veri tabanında bulunan verilere yetkisiz erişme yöntemidir.
Bu güvenlik açığı, normalde görülmesi imkânsız verilerin görüntülenmesine izin verir.
Bir müşteri sisteme girerken kendi kullanıcı adı ve şifresini vererek, giriş izni alır. Bu izin sadece kendi verilerini görmeye yetki verir. 
SQL injection yönteminde ise bir saldırgan bununla birlikte diğer kullanıcıların ve web uygulamasının diğer verilerine erişebilir. 
Buradaki SQL injection açığı ile saldırgan verileri transfer edebilir, değiştirebilir, silebilir. Yani eriştiği tüm verileri manipüle edebilir hale gelir. 
*/
#endregion

#region ExecuteSqlRaw
//Veritabanına yapılacak Insert, Update ve Delete sorgularını yazdığımız bir metottur. Bu metotta ise sorguyu SQL Injection saldırılarına karşı koruma görevi
//geliştirinin sorumluluğundadır.
/*
string name = Console.ReadLine();
var result = context.Database.ExecuteSqlRaw($"INSERT Persons VALUES('{name}')");
//Artık parametre arkaplanda korumaya tabii tutulmayacak. 
*/
#endregion

#region SqlQuery
//SqlQuery fonksiyonu her ne kadar erişilebilir olsada artık desteklenmemektedir. Bunun yerine DbSet propertysi üzerinden erişilebilen FromSql fonksiyonu
//gelmiştir/kullanılmaktadır. RIP.
#endregion

#region SqlQueryRaw
//SqlQueryRaw fonksiyonu her ne kadar erişilebilir olsada artık desteklenememktedir.
//Bunun yerine DbSet propertysi üzerinden erişilebilen FromSqlRaw fonksiyonu gelmiştir/kullanılmaktadır.
#endregion

#region GetMigrations
//Uygulamada üretilmiş olan tüm migration'ları runtime'da programatik olarak elde etmemizi sağlayan metottur.
/*
var migs = context.Database.GetMigrations();
foreach (var item in migs)
{
    Console.WriteLine(item); //bütün migration'ları elde etmiş olduk.
}
*/
#endregion

#region GetAppliedMigrations
//Uygulamada migrate edilmiş olan tüm migrationları elde etmemizi sağlayan bir fonksiyondur.
/*
var migs = context.Database.GetAppliedMigrations();
foreach (var item in migs)
{
    Console.WriteLine(item); //bütün migrate edilmiş migration'ları elde etmiş olduk.
}
*/
#endregion

#region GetPendingMigrations
//Uygulamada migrate edilmemiş olan tüm migrationları elde etmemizi sağlayan bir fonksiyondur.
/*
var migs = context.Database.GetPendingMigrations();
foreach (var item in migs)
{
    Console.WriteLine(item); //uygulanmayan migration'ları elde etmiş olduk.
}
*/
#endregion

#region Migrate
//Migration'ları programatik olarak runtime'da migrate etmek için kullanılan bir fonksiyondur.
//context.Database.Migrate(); //çağırdığımızda uygulamada ne kadar migration varsa migrate edecektir.

//EnsureCreated fonksiyonu migration'ları kapsamamaktadır. O yüzden migraton'lar içerisinde yapılan çalışmalar ilgili fonksiyonda geçerli olmayacaktır.
#endregion

#region OpenConnection
//Veritabanı bağlantısını manuel açar.
//context.Database.OpenConnection();
#endregion

#region CloseConnection
//Veritabanı bağlantısını manuel kapatır.
//context.Database.CloseConnection();
#endregion

#region GetConnectionString
//İlgili context nesnesinin o anda kullandığı ConnectionString değeri ne ise onu elde etmenizi sağlar.
//Console.WriteLine(context.Database.GetConnectionString());
//Ekrana "Server = PC\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True" yazar.
#endregion

#region GetDbConnection
//EF Core'un kullanmış olduğu Ado.NET altyapısının kullandığı DbConnection nesnesini elde etmemizi sağlayan bir fonksiyondur.
//Yani bizi Ado.NET kanadına götürür.
/*
SqlConnection connection = (SqlConnection)context.Database.GetDbConnection();
//connection. diyerek AdoNet'te yapmış olduğumuz tüm çalışmaları gerçekleştirebiliriz. EF Core'u aradan çıkarıp conneciton nesnesini üzerinden temel seviye
//çalışmalarımızı yapabiliriz.
//Kullandığımız veritabanın connection yapılanması nesnesini bize getiren ve sonra da onun üzerinden ADO Net mimarisini kullanarak çalışma gerçekleştirmemizi
//sağlayan bir fonksiyondur. 
Console.WriteLine();
*/
#endregion

#region SetDbConnection
//Kendimize göre özelleştirilmiş olduğumuz connection nesnelerini EF Core mimarisine dahil etmemizi sağlayan bir fonksiyondur. AhmetConnection gibi.
//context.Database.SetDbConnection();
#endregion

#region ProviderName Property'si
//EF Core'un kullanmış olduğu provider neyse onun bilgisini getiren bir proeprty'dir.
//Console.WriteLine(context.Database.ProviderName);
//Microsoft.EntityFrameworkCore.SqlServer yazar.
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");
    }
}
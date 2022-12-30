using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Connection Resiliency Nedir?
//EF Core üzerinde yapılan veritabanı çalışmaları sürecinde ister istemez veritabanı bağlantısında kopuşlar/kesintiler
//vs. meydana gelebilmektedir. 

//Connection Resiliency ile kopan bağlantıyı tekrar kurmak için gerekli tekrar bağlantı taleplerinde bulunabilir ve
//bir yandan da execution strategy dediğimiz davranış modellerini belirleyerek bağlantıların kopması durumunda tekrar
//edecek olan sorguları baştan sona yeniden tetikleyebiliriz.
#endregion

#region EnableRetryOnFailure
//Uygulama sürecinde veritabanı bağlantısı koptuğu taktirde bu yapılandırma sayesinde bağlantıyı tekrardan kurmaya
//çalışabiliyoruz.

while (true) //iki saniyede 1 ekrana person isimlerini yazsın.
{
    await Task.Delay(2000);
    var persons = await context.Persons.ToListAsync();
    persons.ForEach(p => Console.WriteLine(p.Name));
    Console.WriteLine("*******************");
}
//Bağlantı kesintisinde uygulama bir hata fırlatacaktır. Biz bu hata fırlatılmasından ziyade bağlantının yeniden
//sağlanmaya çalışılmasını isteyebiliriz.
//Context'i kurcalayalım.

#region MaxRetryCount
//Yeniden bağlantı sağlanması durumunun kaç kere gerçekleştirleceğini bildirmektedir.
//Defualt değeri 6'dır.
#endregion

#region MaxRetryDelay
//Yeniden bağlantı sağlanması periyodunu bildirmektedir.
//Default değeri 30'dur.
#endregion
#endregion

#region Execution Strategies
//EF Core ile yapılan bir işlem sürecinde veritabanı bağlantısı koptuğu taktirde yeniden bağlantı denenirken yapılan
//davranışa/alınan aksiyona Execution Strategy denmektedir.

//Bu stratejiyi default değerlerde kullanabieceğimiz gibi custom olarak da kendimize göre özelleştirebilir ve
//bağlantı koptuğu durumlarda istediğimiz aksiyonları alabiliriz.

#region Default Execution Strategy
//Eğer ki Connection Resiliency için EnableRetryOnFailure metodunu kullanıyorsak bu default execution stratgy'e
//karşılık gelecektir.
//MaxRetryCoun : 6
//Delay : 30
//Default değerlerin kullanılabilmesi için EnableRetryOnFailure metodunun parametresiz overload'ının kullanılması gerekmektedir.
#endregion

#region Custom Execution Strategy

#region Oluşturma
//Görevi üstlenecek bir classs oluşturmakla göreve başlayalım. (en altta)
#endregion

#region Kullanma - ExecutionStrategy

//while (true)
//{
//    await Task.Delay(2000);
//    var persons = await context.Persons.ToListAsync();
//    persons.ForEach(p => Console.WriteLine(p.Name));
//    Console.WriteLine("*******************");
//}
#endregion

#endregion
#region Bağlantı Koptuğu Anda Execute Edilmesi Gereken Tüm Çalışmaları Tekrar İşlemek

//EF Core ile yapılan çalışma sürecinde veritabanı bağlantısının kesildiği durumlarda, bazen bağlantının tekrardan
//kurulması tek başına yetmemekte, kesintinin olduğu çalışmanın da baştan tekrardan işlenmesi gerekebilmetkedir.
//İşte bu tarz durumlara karşılık EF Core Execute - ExecuteAsync fonksiyonunu bizlere sunmaktadır.

//Execute fonksiyonu, içerisine vermiş olduğumuz kodları commit edilene kadar işleyecektir.
//Eğer ki bağlantı kesilmesi meydana gelirse, bağlantının tekrardan kurulması durumunda Execute içerisindeki çalışmalar
//tekrar baştan işlenecek ve böylece yapılan işlemin tutarlılığı için gerekli çalışma sağlanmış olacaktır.
/*
var strategy = context.Database.CreateExecutionStrategy(); //o anki ExecutionStrategy Instance'ını elde ediyoruz.
await strategy.ExecuteAsync(async () => 
{
    //iki tane person ekleyen bir transaction işlemi gerçekleştirelim.
    using var transcation = await context.Database.BeginTransactionAsync();

    await context.Persons.AddAsync(new() { Name = "Ahmet" });
    await context.SaveChangesAsync();

    await context.Persons.AddAsync(new Person() { Name = "Çağatay" });
    await context.SaveChangesAsync();

    await transcation.CommitAsync(); //transaction'umuzu commit'leyemeden veri tabanı bağlantısı gümlerse
    //aşağıdaki stratejimize göre tekrar bağlantılar denenecek, tekrar bağlantı kurulduğunda yukarıdaki işlemlerin
    //baştan bir daha işlenmesini istiyorsak, ExecuteAsync kullanmamız gereklidir. Yarım kalan süreç baştan başlayacaktır.
});
*/
#endregion

#region Execution Strategy Hangi Durumlarda Kullanılır?
//Örnek olarak;
//Veritabanının şifresi belirli periyotlarda otomatik olarak değişen uygulamalarda güncel şifreyle connection string'i
//sağlayacak bir operasyonu custom execution strategy belirleyerek gerçekleştirebiliriz.
#endregion
#endregion

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //EnableRetryOnFailure() ile,
        //bir kesinti meydana gelirse default ayarlar ile bu kesintiye karşılık bağlantıyı tekrardan sağla.
        //30 saniyede 1, altı kez bağlantıyı dene. Toplam 180 saniyeden sonra kesinti giderilmezse hatayı verir.
        /*
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; " +
            "TrustServerCertificate = True", builder => builder.EnableRetryOnFailure());
        */
        /*
        //Yeniden bağlantının kurulmaya çalışıldığı durumlarda ekranda bilgilendirme yazısı istiyorsak;
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; " +
            "TrustServerCertificate = True", builder => builder.EnableRetryOnFailure())
            .LogTo( //sadece yeniden bağlantı durumlarına karşılık gelecek Log'ları filtreleyebiliriz.
            filter: (eventId, level) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
            logger: EventData =>
            {
                Console.WriteLine($"Bağlantı yeniden kuruluyor.");
            });
        */

        #region Default Execution Strategy
        /*
        //max tekrar ve delay'leri belirleyelim.
        //errorNumbersToAdd
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; " +
        "TrustServerCertificate = True", builder => builder.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorNumbersToAdd: new[] { 4060 }))
            .LogTo(
            filter: (eventId, level) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
            logger: eventData =>
            {
                Console.WriteLine($"Bağlantı tekrar kurulmaktadır.");
            });
        */
        #endregion
        #region Custom Execution Strategy
        /*
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1" +
            ";TrustServerCertificate = True", builder => builder.ExecutionStrategy(
            //custom olan Execution Strategy'i devreye sokmak istiyorsak ExecutionStrategy fonksiyonunu kullanmalıyız.
            dependencies => new CustomExecutionStrategy(dependencies, 10, TimeSpan.FromSeconds(15))));
            //constructor'lara gerekli parametreleri verdikten sonra işlem tamam. 
        */
        #endregion
    }
}

//Custom Execution S.'nin kullanılabilmesi için öncelikle bir class açalım.
class CustomExecutionStrategy : ExecutionStrategy //ExecutionStrategy sınıfından kalıtım almalıdır.
{//ExecutionStrategy bir base class görevi gördüğü için içerisindeki constructorkar'da parametre aldığından burada otomatik
    //oluşturup bu parametlere karşılık değerlerini de vermemiz gerekecektir.
//ilgili parametrelerin değerlerini burada base keyword'ü ile base class'ın constructor'larına gönderiyoruz.
    public CustomExecutionStrategy(ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay) 
            : base(dependencies, maxRetryCount, maxRetryDelay)
    {
    }

    public CustomExecutionStrategy(DbContext context, int maxRetryCount, TimeSpan maxRetryDelay) 
            : base(context, maxRetryCount, maxRetryDelay)
    {
    }
//sonraki yapılmak istenilen işlemler ShouldRetyOn()'da...

    int retryCount = 0; //Ekrana 1. Bağlantı... 2. Bağlantı yen.... tarzı şovlar da yapabiliriz.
    protected override bool ShouldRetryOn(Exception exception)
    {
    //Yeniden bağlantı durumunun söz konusu olduğu anlarda yapılacak işlemler...
    Console.WriteLine($"#{++retryCount}. Bağlantı tekrar kuruluyor...");
    return true;
    }
}
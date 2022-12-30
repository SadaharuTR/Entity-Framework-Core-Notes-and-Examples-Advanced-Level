
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata;

ApplicationDbContext context = new();
var datas = await context.Persons.ToListAsync();

#region Neden Loglama Yaparız?
//Çalışan bir sistemin runtime'da nasıl davranış gerçekleştirdiğini gözlemleyebilmek için log mekanizmalarından istifade ederiz.
#endregion

#region Neleri Loglarız?
//Yapılan sorguların çalışma süreçlerindeki davranışlarını.
//Gerekirse hassas veriler üzerinde de loglama işlemleri gerçekleştiriyoruz.
#endregion

#region Basit Olarak Loglama Nasıl Yapılır?
//Minumum yapılandırma gerektirmesi açısından basit olarak loglama yapabiliriz.
//Herhangi bir nuget paketine ihtiyaç duyulmaksızın loglamanın yapılabilmesi.
//Log işlemleri OnConfiguring içerisinde yapılmaktadır.

#region Debug Penceresine Log Nasıl Atılır?
//Debug.WriteLine ile kullanmış olduğumuz IDE'nin Debug penceresine Log'larız.
#endregion

#region Bir Dosyaya Log Nasıl Atılır?
//Normalde Console yahut Debug pencerelerine atılan loglar pek takip edilebilir nitelikte olmamaktadır.
//Logları kalıcı hale getirmek istediğimiz durumlarda en basit halyile bu logları harici bir dosyaya atmak isteyebiliriz. (genelde txt olur)
#endregion

#endregion

#region Hassas Verilerin Loglanması - EnableSensitiveDataLogging
//Default olarak EF Core log mesajlarında herhangi bir verinin değerini içermemektedir. Bunun nedeni, gizlilik teşkil edebilecek verilerin loglama
//sürecinde yanlışlıkla dahi olsa açığa çıkmamasının istenmesidir. 
//Bazen alınan hatalarda verinin değerini bilmek hatayı debug edebilmek için oldukça yardımcı olabilmektedir.
//Bu durumda hassas verilerinde loglamasını sağlayabiliriz.
#endregion

#region Exception Ayrıntısını Loglama - EnableDetailedErrors
//Hataları da log'layabiliriz. Detaylı olup olmayacağı bize kalmıştır.
#endregion

#region Log Levels
//EF Core default olarak Debug seviyesinin üstündeki (debug dahil) tüm davranışları loglar. Eğer ki buna müdahele etmek istiyorsak,
//optionsBuilder.LogTo(async message => await _log.WriteLineAsync(message), LogLevel.Information) 'da olduğu gibi LogLevel.Information'ı ikinci parametre olarak yazarak,
//Debug değil Information'dan sonrasını Log'la diyebiliriz.

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
    //Harici txt dosyasına log atmak için StreamWriter sınıfından faydalanıyoruz. Uygulamanın debug klasörüne atar. Append ile de üzerine ekleme operasyonuna
    //devam etmesini söylüyoruz.
    StreamWriter _log = new("logs.txt", append: true);
    
    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");

        //optionsBuilder.LogTo(Console.WriteLine); //Logto metodunu kullanarak CW ile loglama'yı gerçekleştir.

        //optionsBuilder.LogTo(message => Debug.WriteLine(message)); //Debug penceresine log'la.

        //optionsBuilder.LogTo(async message => await _log.WriteLineAsync(message)); //ile harici dosyamıza log'umuzu atabiliriz.

        //Hangi log'lama çeşidini kullanıyorsak kullanalım EnableSensitiveDataLogging()'i çağırmamız hassas dataların gösterilmesi için yeterli olacaktır.
        /*
        optionsBuilder.LogTo(async message => await _log.WriteLineAsync(message))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors(); //'i çağırarak hataları daha detaylı görebiliriz.
        */

        //
        optionsBuilder.LogTo(async message => await _log.WriteLineAsync(message), LogLevel.Information) //'da olduğu gibi LogLevel.Information'ı ikinci parametre olarak yazarak,
        //Debug değil Information'dan sonrasını Log'la diyebiliriz.
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors(); 

    }

    //Stream yapılanmaları kullanırken bunları bir şekilde kapatmayı unutmayalım.
    //Dispose fonksiyonları ile bunu sağlayabiliriz.
    public override void Dispose()
    {
        base.Dispose();
        _log.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _log.DisposeAsync();
    }
    
}
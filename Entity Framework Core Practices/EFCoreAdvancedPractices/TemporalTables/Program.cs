using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;

ApplicationDbContext context = new();

//Önce oku: https://www.gencayyildiz.com/blog/sql-server-2016-temporal-tables/

#region Temporal Tables Nedir?
//Veri değişikliği süreçlerinde kayıtları depolayan ve zaman içinde farklı noktalardaki tablo verilerinin analizi için kullanılan ve sistem tarafından yönetilen tablolardır.
//EF Core 6.0 ile desteklenmektedir.
#endregion

#region Temporal Tables Özelliğiyle Nasıl Çalışılır?
//EF Core'daki migration yapıları sayesinde Temporal Table'lar oluşturulup veritabanında üretilebilmekteyiz.
//Mevcut tabloları migration'lar aracılığıyla Temporal Table'lara dönüştürülebilmekteyiz.
//Herhangi bir tablonun verisel olarak geçmişini rahatlıkla sorgulayabiliriz.
//Herhangi bir tablodaki bir verinin geçmişteki herhangi bir T anındaki hali/durumu/verileri geri getirilebilmektedir.
#endregion

#region Temporal Table Nasıl Uygulanır?

#region IsTemporal Yapılandırması
//EF Core bu yapılandırma fonksiyonu sayesinde ilgili Entity'e karşılık üretilecek tabloda temporal table oluşturacağını anlamaktadır.
//Yahut önceden ilgili tablo üretilmişse eğer onu Temporal Table'a dönüştürecektir.
#endregion

#region Temporal Table İçin Üretilen Migration'ın İncelenmesi

#endregion
#endregion
#region Temporal Table'ı Test Edelim

#region Veri Eklerken
//Temporal Table'a veri ekleme süreçlerinde herhangi bir kayıt atılmaz! Temporal Table'ın yapısı, var olan veriler üzerindeki zamansal değişimleri takip etmek üzerine kuruludur!
/*
var persons = new List<Person>() {
    new(){ Name = "Dimitri", Surname = "Antonyanidis", BirthDate = DateTime.UtcNow },
    new(){ Name = "Giannis", Surname = "Antonyanidis", BirthDate = DateTime.UtcNow },
    new(){ Name = "Semiramis", Surname = "Antonyanidis", BirthDate = DateTime.UtcNow },
    new(){ Name = "Dimitriadis", Surname = "Antonyanidis", BirthDate = DateTime.UtcNow },
    new(){ Name = "Disdis", Surname = "Antonyanidis", BirthDate = DateTime.UtcNow },
    new(){ Name = "Mahmut", Surname = "Antonyanidis", BirthDate = DateTime.UtcNow }
};

await context.Persons.AddRangeAsync(persons); //bu kayıtları veritabanına ekleyelim.
await context.SaveChangesAsync();
*/
//PersonHistory'de bir değişiklik olmaz.
#endregion

#region Veri Güncellerken
/*
var person = await context.Persons.FindAsync(3);
person.Name = "Ahmet";
await context.SaveChangesAsync();
*/
#endregion

#region Veri Silerken
/*
var person = await context.Persons.FindAsync(3);
context.Persons.Remove(person);
await context.SaveChangesAsync();
*/
//Güncelleme ve silme işlemlerinde verinin önceki halini PersonHistory'de görebiliriz.
#endregion
#endregion

#region Temporal Table Üzerinden Geçmiş Verisel İzleri Sorgulama

#region TemporalAsOf
//Belirli bir zaman için değişikiğe uğrayan tüm öğeleri döndüren bir fonksiyondur.
//2022-12-27 01:11:12.7141823
/*
var datas = await context.Persons.TemporalAsOf(new DateTime(2022, 12, 27, 01, 11, 58)).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"), //PeriodStart ve PeriodEnd arka planda Shadow Property olarak tutulmaktadır.
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd"), //EF.Property ile çağırırız.
}).ToListAsync();

foreach (var data in datas)
{
    Console.WriteLine(data.Id + " " + data.Name + " | " + data.PeriodStart + " - " + data.PeriodEnd);
}
*/
/*
1 Dimitri | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
2 Giannis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
4 Dimitriadis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
5 Disdis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
6 Mahmut | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
3 Semiramis | 27.12.2022 01:11:12 - 27.12.2022 01:11:58
*/
//verilen tarihte o an değişikliğe uğrayan Semiramis ile diğer değerlerde hiçbir değişiklik yoksa neyse onu getirmiş oldu.
//vermiş olduğumuz tarihteki veriler bütünsel olarak bunlarmış.
#endregion

#region TemporalAll
//Güncellenmiş yahut silinmiş olan tüm verilerin geçmiş sürümlerini veya geçerli durumlarını döndüren bir fonksiyondur.
/*
var datas = await context.Persons.TemporalAll().Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd"),
}).ToListAsync();

foreach (var data in datas)
{
    Console.WriteLine(data.Id + " " + data.Name + " | " + data.PeriodStart + " - " + data.PeriodEnd);
}
*/
/* Ne var ne yok elde ettik. 9999 ile bitenlerde bir değişiklik olmamıştır. 9999'a karşılık 3 Id'li bir veri yok, demek ki silinmiş. 2022 ile bitenler de güncellenmiş.
1 Dimitri | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
2 Giannis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
4 Dimitriadis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
5 Disdis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
6 Mahmut | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
3 Semiramis | 27.12.2022 01:11:12 - 27.12.2022 01:11:58
3 Ahmet | 27.12.2022 01:11:58 - 27.12.2022 01:15:31
*/
//SQL yapılandırmasında kullanılan DateTime2, 9999'da bitiyor. (Genel Kültür)
#endregion

#region TemporalFromTo
//Belirli bir zaman aralığı içerisindeki verileri döndüren fonksiyondur. Başlangıç ve bitiş zamanı dahil değildir.
/*
////Başlangıç : 2022-12-27 01:11:58.6130795
var baslangic = new DateTime(2022, 12, 27, 01, 11, 31);
////Bitiş     : 2022-12-27 01:15:31.2039901
var bitis = new DateTime(2022, 12, 27, 01, 11, 58);

var datas = await context.Persons.TemporalFromTo(baslangic, bitis).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd"),
}).ToListAsync();

foreach (var data in datas)
{
    Console.WriteLine(data.Id + " " + data.Name + " | " + data.PeriodStart + " - " + data.PeriodEnd);
}
*/
/*
1 Dimitri | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
2 Giannis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
4 Dimitriadis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
5 Disdis | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
6 Mahmut | 27.12.2022 01:11:12 - 31.12.9999 23:59:59
3 Semiramis | 27.12.2022 01:11:12 - 27.12.2022 01:11:58 
*/
#endregion

#region TemporalBetween
////Belirli bir zaman aralığı içerisindeki verileri döndüren fonksiyondur. Başlangıç verisi dahil değil ve bitiş zamanı ise dahildir.
/*
//Başlangıç : 2022-12-27 01:11:58.6130795
var baslangic = new DateTime(2022, 12, 27, 01, 11, 31);
//Bitiş     : 2022-12-27 01:15:31.2039901
var bitis = new DateTime(2022, 12, 27, 01, 11, 58);

var datas = await context.Persons.TemporalBetween(baslangic, bitis).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd"),
}).ToListAsync();

foreach (var data in datas)
{
    Console.WriteLine(data.Id + " " + data.Name + " | " + data.PeriodStart + " - " + data.PeriodEnd);
}
*/
#endregion

#region TemporalContainedIn
//Belirli bir zaman aralığı içerisindeki verileri döndüren fonksiyondur. Başlangıç ve bitiş zamanı ise dahildir. Tarihler tam dahil olduğu için değişikliği elde etmemizi sağlar.
/*
//Başlangıç : 2022-12-27 01:11:58.6130795
var baslangic = new DateTime(2022, 12, 27, 01, 11, 58);
//Bitiş     : 2022-12-27 01:15:31.2039901
var bitis = new DateTime(2022, 12, 27, 01, 11, 27);

var datas = await context.Persons.TemporalContainedIn(baslangic, bitis).Select(p => new
{
    p.Id,
    p.Name,
    PeriodStart = EF.Property<DateTime>(p, "PeriodStart"),
    PeriodEnd = EF.Property<DateTime>(p, "PeriodEnd"),
}).ToListAsync();

foreach (var data in datas)
{
    Console.WriteLine(data.Id + " " + data.Name + " | " + data.PeriodStart + " - " + data.PeriodEnd);
}
*/
#endregion
#endregion

#region Silinmiş Bir Veriyi Temporal Table'dan Geri Getirme
//Silinmiş bir veriyi temporal table'dan getirebilmek için öncelikle yapılması gereken bilgili verinin silindiği tarihi bulmamız gerekmektedir.
//Ardından TemporalAsOf fonksiyonu ile silinen verinin geçmiş değeri elde edilebilir ve fiziksel tabloya bu veri taşınabilir.

//Silindiği Tarih
var dateOfDelete = await context.Persons.TemporalAll()
    .Where(p => p.Id == 3)
    .OrderByDescending(p => EF.Property<DateTime>(p, "PeriodEnd")) //PeriodEnd'e göre bir Desc sıralama.
    .Select(p => EF.Property<DateTime>(p, "PeriodEnd")) //gelen verilerden PeriodEnd'i çek.
    .FirstAsync(); //gelen verilerden ilkini ver. Çünkü Desc ile sorgulama sürecinde başa aldık.

//Silinen Veri
var deletedPerson = await context.Persons.TemporalAsOf(dateOfDelete.AddMilliseconds(-1)) //1 milisaniye öncesini alalım ki aradığımız veri gelsin.
    .FirstOrDefaultAsync(p => p.Id == 3); //Id'si 3 olan gelsin.

await context.AddAsync(deletedPerson); //ilgili veriyi veritabanına gönder.

await context.Database.OpenConnectionAsync(); 

await context.Database.ExecuteSqlInterpolatedAsync($"SET IDENTITY_INSERT dbo.Persons ON"); //Person tablosu üzerinde aktifleştir.
await context.SaveChangesAsync(); //'dan önce bu veriyi ekleyebilmek için IDENTITY_INSERT konf.'unu aktifleştirmemiz gerek. Yoksa patlar.
await context.Database.ExecuteSqlInterpolatedAsync($"SET IDENTITY_INSERT dbo.Persons OFF"); //Sonrada kapatıyoruz. Öyle her önüne gelen hayırdır.

#region SET IDENTITY_INSERT Konfigürasyonu
//Id ile veri ekleme sürecinde ilgili verinin Id sütununa kayıt işleyebilmek için veriyi fiziksel tabloya taşıma işleminden önce veritabanı seviyesinde
//SET IDENTITY_INSERT komutu eşliğinde Id bazlı veri ekleme işlemi aktifleştirilmelidir.
#endregion
#endregion

class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
}
class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().ToTable("Employees", builder => builder.IsTemporal()); //bu tablonun Temporal olup olmayacağının konf. burada gerçekleştiriyoruz.
        modelBuilder.Entity<Person>().ToTable("Persons", builder => builder.IsTemporal()); //var olan bir tabloyu da sonradan temporal hale getirebiliriz.
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");
    }
}
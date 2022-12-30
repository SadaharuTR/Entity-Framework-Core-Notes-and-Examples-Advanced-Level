using Microsoft.EntityFrameworkCore;

ApplicationDbContext context = new();

#region Table Per Hierarchy (TPH) Nedir?
//Kalıtımsal ilişkiye sahip olan Entity'lerin olduğu senaryolarda her bir hiyerarşiye karşılık bir tablo oluşturan davranıştır.
#endregion

#region Neden Table Per Hierarchy Yaklaşımında Bir Tabloya İhtiyacımız Olsun?
//İçerisinde benzer alanlara sahip olan entityleri migrate ettiğimizde, her Entity'e karşılık bir tablo oluşturmaktansa bu entityleri tek bir tabloda modellemek isteyebilir
//ve bu tablodaki kayıtları discriminator kolonu üzerinden birbirlerinden ayırabiliriz. İşte bu tarz bir tablonun oluşturulması ve bu tarz bir tabloya göre sorgulama, veri ekleme,
//silme vs. gibi operasyonların şekillendirilmesi için TPH davranışını kullanabiliriz.
#endregion

#region TPH Nasıl Uygulanır?
//EF Core'da Entity arasında temel bir kalıtımsal ilişki söz konusuysa eğer default olarak kabul edilen davranıştır.
//O yüzden herhangi bir konfigürasyon gerektirmez!

//TPH'nin uygulanması için tek yapılması gereken;
//Entity'ler kendi aralarında kalıtımsal ilişkiye sahip olmalı ve bu Entity'lerin hepsi DbContext nesnesine DbSet olarak eklenmelidir! 
#endregion

#region Discriminator Kolonu Nedir?
//Table Per Hierarchy yaklaşımı neticesinde kümülatif olarak inşa edilmiş tablonun hangi Entity'e karşılık veri tuttuğunu ayırt edebilmemizi sağlayan bir kolondur.
//EF Core tarafından otomatik olarak tabloya yerleştirilir. Default olarak içerisinde Entity isimlerini tutar. Discriminator kolonunu istediğimiz gibi özelleştirebiliriz.
#endregion

#region Discriminator Kolon Adı Nasıl Değiştirilir?
//Öncelikle hiyerarşinin başında hangi sınıf varsa onun Fluent API'da konfigürasyonuna gidilmeli ve ardından HasDiscriminator fonksiyonu ile özelleştirilmelidir.
/*
//Bu eklemeyi yaptığımızda Discriminator kolonunun adının AyiranYuvaYikan olduğunu ve içindeki değerin Employee yazdığını görürürüz.
Employee employee = new() { Name = "Mahmut", Surname = "Tuncer", Department = "Müzik Tanrısı" };
await context.Employees.AddAsync(employee);
await context.SaveChangesAsync();
*/
#endregion

#region Discriminator Değerleri Nasıl Değiştirilir?
//Yine hiyerarşinin başındaki entity konfigürasyonlarına gelip, HasDiscriminator fonksiyonu ile özelleştirmede bulunarak ardından HasValue fonksiyonu ile
//hangi entitye karşılık hangi değerin girileceğini belirtilen türde ifade edebiliriz.
/*
//Bu eklemeyi yaptığımızda Discriminator kolonunun adının AyiranYuvaYikan olduğunu ve içindeki değerin B yazdığını görürürüz. Çünkü HasValue ile B olmasını istedik.
Employee employee = new() { Name = "Mahmut", Surname = "Tuncer", Department = "Müzik Tanrısı" };
await context.Employees.AddAsync(employee);
await context.SaveChangesAsync();
*/
#endregion

#region TPH'da Veri Ekleme

//Davranışların hiçbirinde veri eklerken,silerken, güncellerken vs. normal operasyonların dışında bir işlem yapılmaz!
//Hangi davranışı kullanıyorsak, EF Core ona göre arkaplanda modellemeyi gerçekleştirecektir.
/*
Employee e1 = new() { Name = "Trent", Surname = "Alexander-Arnold", Department = "Sağ Bek" };
Employee e2 = new() { Name = "Virgil", Surname = "Van Dijk", Department = "Savunma Bakanı" };
Customer c1 = new() { Name = "Muhammed", Surname = "Salah", CompanyName = "Ofansif Kanat AŞ." };
Customer c2 = new() { Name = "Ferdi", Surname = "Tayfur", CompanyName = "Damar Bakanlığı" };
Technician t1 = new() { Name = "Müslüm", Surname = "Gürses", Department = "Damar Tanrısı", Branch = "Damarcı" };

await context.Employees.AddAsync(e1);
await context.Employees.AddAsync(e2);
await context.Customers.AddAsync(c1);
await context.Customers.AddAsync(c2);
await context.Technicians.AddAsync(t1);

await context.SaveChangesAsync();
*/
#endregion

#region TPH'da Veri Silme
//TPH davranışında silme operasyonu yine Entity üzerinden gerçekleştirilir.
//var employee = await context.Employees.FindAsync(3); //Employee'lerden 3 Id'sine karşılık olan veriyi sildik.
//Burada 3 Id'sine sahip veriyi Persons'tan istemiyoruz. 
//3 Id'sine sahip olan ve Emplyee olan veriyi istiyoruz. Discriminator üzerinden bir sorgu yapılıp,
//Employee'a karşılık 3 Id'si varsa onu getirir. Yoksa hata verir.

//tüm customer'ları silelim
//var customers = await context.Customers.ToListAsync();
//context.Customers.RemoveRange(customers);
//await context.SaveChangesAsync();

//context.Employees.Remove(employee);
//await context.SaveChangesAsync();

#endregion

#region TPH'da Veri Güncelleme
//TPH davranışında güncelleme operasyonu yine entity üzerinden gerçekleştirilir.
Employee guncellenecek = await context.Employees.FindAsync(8);
guncellenecek.Name = "Kerpeten Ali";
await context.SaveChangesAsync();
#endregion

#region TPH'da Veri Sorgulama
//Veri sorgulama oeprasyonu bilinen DbSet propertysi üzerinden sorgulamadır. Ancak burada dikkat edilmesi gereken bir husus vardır;

var employees = await context.Employees.ToListAsync(); //böyle bir sorgulama yapıyorsak kalıtımsal bağ olduğundan dolayı Technician'lar da gelecektir.
var techs = await context.Technicians.ToListAsync(); //bu sorguyu sadece Technician'lar için yaparsak sadece Technician'lar gelir.

//kalıtımsal ilişkiye göre yapılan sorgulamada üst sınıf alt sınıftaki verileride kapsamaktadır.
//O yüzden üst sınıfların sorgulamalarında alt sınıfların verileride gelecektir buna dikkat edilmelidir.
//Sorgulama süreçlerinde EF Core generate edilen sorguya bir where şartı eklemektedir. TPH davranışının getirisi.
#endregion

#region Farklı Entity'ler de Aynı İsimde Sütunların Olduğu Durumlar
//Entity'lerde tekrar eden kolonlar olabilir. Bu kolonları EF core isimsel olarak özelleştirip ayıracaktır.
//Diyelim ki Customer'da da Technician'da da A isimli bir sürun olsun. Migrate edildiğinde birine normal A ismini verirken diğerine entity ismiyle beraber bir özelleştirme
//yapıp Technician_A ya da TechnicianA yapacaktır. Kalıtım derecesine göre orjinal isim ataması yapıyor gibi gözüküyor. Customer derece olarak daha yukarıda o yüzden kolonu
//orjinal A ismini alıyor.
#endregion
abstract class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
}
class Employee : Person
{
    public string? Department { get; set; }
}
class Customer : Person
{
    public int A { get; set; }
    public string? CompanyName { get; set; }
}
class Technician : Employee
{
    public int A { get; set; }
    public string? Branch { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Technician> Technicians { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /* Discriminator İşlemleri için;
        //önce hiyerarşi'nin başı, base class'ı, reisi olan Person'a gidilmeli. 
        modelBuilder.Entity<Person>()
            .HasDiscriminator<string>("AyiranYuvaYikan") //HasDiscriminator kolonunun ismi AyiranYuvaYikan olsun türü de string olsun.
            //.HasDiscriminator<int>("AyiranYuvaYikan"); integer'da olabilir.
            .HasValue<Person>("A") //Person geliyorsa A olsun. Ya da int yaptıysak 1 olsun diyebiliriz.
            .HasValue<Employee>("B") //Employee geliyorsa B, int ise 2,
            .HasValue<Customer>("C") //Customer geliyorsa C, int ise 3,
            .HasValue<Technician>("D"); //Teknisyen geliyorsa D olsun. int ise 4 olsun diyebiliriz.
        */
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
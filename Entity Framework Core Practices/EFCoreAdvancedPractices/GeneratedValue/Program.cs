using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

ApplicationDbContext context = new();

#region Generated Value Nedir?
//EF Core'da üretilen değerlerle ilgili çeşitli modellerin ayrıntılarını yapılandırmamızı sağlayan bir yapılandırmadır.
#endregion

#region Default Values

//EF Core'da herhangi bir tablonun herhangi bir kolonuna yazılım tarafından bir değer gönderilmediği taktirde bu kolona hangi değerin(default value olarak) üretilip
//yazdırılacağını belirleyen yapılanmalardır.

#region HasDefaultValue
//Burada direkt Static veri veriyoruz.
#endregion

#region HasDefaultValueSql
//Burada ise SQL cümleciği veriyoruz.
#endregion

#endregion

#region Computed Columns

#region HasComputedColumnSql
//Tablo içerisindeki kolonlar üzerinde yapılan aritmatik işlemler neticesinde üretilen kolondur.
#endregion

#endregion

#region Value Generation

#region Primary Keys
//Herhangi bir tablodaki satırları kimlik vari şekilde tanımlayan, tekil(unique) olan sütun veya sütunlardır.
#endregion

#region Identity
//Identity, yalnızca otomatik olarak artan bir sütundur. Bir sütun, PK olmaksızın identity olarak tanımlanabilir.
//Bir tablo içerisinde identity kolonu sadece tek bir tane olarak tanımlanabilir.
#endregion

//Bu iki özellik genellikle birlikte kullanılmaktadırlar. O yüzden EF Core PK olan bir kolonu otomatik olarak Identity olacak şekilde yapılandırmaktadır.
//Ancak böyle olması için bir gereklilik yoktur!

#region DatabaseGenerated

#region DatabaseGeneratedOption.None - ValueGeneratedNever
//Bir kolonda değer üretilmeyecekse eğer None ile işaretliyoruz.
//EF Core'un default olarak PK kolonlarda getirdiği Identity özelliğini kaldırmak istiyorsak eğer None'ı kullanabiliriz.
#endregion

#region DatabaseGeneratedOption.Identity - ValueGeneratedOnAdd
//Bir kolonda değer üretilecekse ve bu değerin ardışık bir şekilde artmasını bekliyorsak .Identity ile işaretleyeceğiz.
//Herhangi bir kolona Identity özelliğini vermemizi sağlayan bir konfigürasyondur.

#region Sayısal Türlerde
//Eğer ki Identity özelliği bir tabloda sayısal olan bir kolonda kullanılacaksa o durumda ilgili tablodaki PK olan kolondan özellikle/iradeli bir şekilde
//identity özelliğinin kaldırılması gerekmektedir.(None)
#endregion

#region Sayısal Olmayan Türlerde
//Eğer ki Identity'de String ya da unique identifier türünde bir değer olarak bildirirsek, o zaman kullanmış olduğumuz veritabanı sunucusunun o türe karşılık özelliği neyse
//ona göre davranış sergileyecektir. Yoksa öyle bir davranışı default değer neyse onu verecektir. 
//Eğer ki Identity özelliği bir tabloda sayısal olmayan bir kolonda kullanılacaksa o durumda ilgili tablodaki PK olan kolondan iradeli bir şekilde
//identity özelliğinin kaldırılmasına gerek yoktur. 
#endregion

#endregion

#region DatabaseGeneratedOption.Computed - ValueGeneratedOnAddOrUpdate
//EF Core üzerinde bir kolon Computed column ise istersek Computed olarak belirleyebiliriz istersek de belirtmeden kullanmaya devam edebilirsiniz.
//Bu seçenekte değerin ilk kaydedildiğinde veritabanı tarafından oluşturulacağını ve daha sonra her veri güncellemesinde de değerin yeniden oluşturulacağını belirtmektedir.
#endregion

#endregion

#endregion
/*
Person p = new()
{
    Name = "Zalarko",
    Surname = "Carrier",
    //Salary = 200, yazarsak buradaki değeri ekleyecek. Yazmazsak default olarak belirlenen 100 değeri ekleyecek.
    Premium = 10,
    TotalGain = 110
};
await context.Persons.AddAsync(p);
await context.SaveChangesAsync();
*/

//DatabaseGeneratedOption.None işlemlerinden sonra veri eklerken;
/*
Person p = new()
{
    PersonId = 1, //artık manuel olarak eklememiz lazım. İdentity özelliğini aldık çünkü. Fakat unique özelliği duruyor.
    Name = "Malarko",
    Surname = "Marrier",
    Premium = 10,
    TotalGain = 110
};
await context.Persons.AddAsync(p);
await context.SaveChangesAsync();
*/
class Person
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] //burada ise PersonId kolonunun identity olma özelliğini düşürüyoruz. Name COnvention gereği zaten PK olarak kalacak.
    public int PersonId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Premium { get; set; }
    public int Salary { get; set; }
    //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] // istiyorsak Computed kolon olduğunu bildirebiliriz.
    public int TotalGain { get; set; }
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] //PersonCode kolonunu identity olarak atamak istiyorsak, PersonID kolonunun identity'liğini düşürmemiz lazım.
    //public int PersonCode { get; set; } //çünkü burada PersonCode int türünde.

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //burada int-sayısal bir değer olmadığı için yukarıda ekstradan PersonId'yi None olarak bildirmeye gerek yok.
    public Guid PersonCode { get; set; }

}

class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Eğer ki Person'a herhangi bir maaş vermiyorsak bu maaş default olarak şu kadar olsun.
        modelBuilder.Entity<Person>()
            .Property(p => p.Salary)
            //.HasDefaultValue(100);
            .HasDefaultValueSql("FLOOR(RAND() * 1000)");

        //aritmetik işlemler sonucu TotalGain kolonuna yazdırılacaktır.
        modelBuilder.Entity<Person>()
            .Property(p => p.TotalGain)
            .HasComputedColumnSql("([Salary] + [Premium]) * 10")
            .ValueGeneratedOnAddOrUpdate(); //TotalGain'in computed kolon olduğunu yukarıdaki satırda bildirdik.
                                            //Ama bir yandan da ValueGeneratedOnAddOrUpdate ile de Fluent API ile tekrar bildirebiliriz. Gerek de yok yani.

        //DatabaseGeneratedOption.None Data Annotation'un Fluent API karşılığı;
        modelBuilder.Entity<Person>()
            .Property(p => p.PersonId)
            .ValueGeneratedNever(); //bana sormadan herhangi bir identity özelliğinde davranış sergileme!

        //eğer ki identity tanımlaması yapıldıysa ve tür sayısal bir değer değilse,
        //bu sefer public Guid PersonCode { get; set; }'e default bir değer vermemiz lazım.
        modelBuilder.Entity<Person>()
            .Property(p => p.PersonCode)
            .HasDefaultValueSql("NEWID()"); //NEWID() fonksiyonu ile SQL cümleciği ile unique identifier bir değer üretip bunu default olarak PersonCode'a verebiliriz.

        //burada PersonCode'un identity olmasını istiyorsak ValueGeneratedOnAdd ile de Fluent API kullanarak bildirebiliriz.
        modelBuilder.Entity<Person>()
            .Property(p => p.PersonCode)
            .ValueGeneratedOnAdd();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region EF Core'da Neden Yapılandırmalara İhtiyacımız Olur?
//EF Core'unda kendine has default davranışları vardır.
//Default davranışları yeri geldiğinde geçersiz kılmak ve özelleştirmek isteyebiliriz. Bundan dolayı yapılandırmalara ihtiyacımız olacaktır.
#endregion

#region OnModelCreating Metodu
/*
EF Core'da yapılandırma deyince akla ilk gelen metot OnModelCreating metodudur.
Bu metot, DbContext sınıfı içerisinde virtual olarak ayarlanmış bir metottur.
Bizler bu metodu kullanarak model'larımızla ilgili temel konfigürasyonel davranışları (Fluent API) sergileyebiliriz.
Bir model'ın yaratılışıyla ilgili tüm konfigürasyonları burada gerçekleştirebilmekteyiz. (Elimizdeki herhangi bir Entity'nin yani model'in
ismi, entiy kolonlarının türleri, davranışları, default değerleri vs.
Migration oluşturulmadan önce OnModelCreating metodu araya girer ve modellerle ilgili temel konfigürasyonel yapılanmaları migration'a sunar. 
*/

#region GetEntityTypes
//EF Core'da kullanılan entity'leri elde etmek, programatik olarak öğrenmek istiyorsak eğer GetEntityTypes fonksiyonunu kullanabiliriz.
#endregion

#endregion

#region Configurations | Data Annotations & Fluent API

//-'nin sol tarafındaki Data Annotations, sağ tarafındaki onun Fluent API karşılığıdır.
#region Table - ToTable
//Generate edilecek tablonun ismini belirlememizi sağlayan yapılandırmadır.
//EF Core normal şartlarda (default olarak) generate edeceği tablonun adını DbSet property'sinden almaktadır.
//Bizler eğer ki bunu özelleştirmek istiyorsak Table attribute'unu yahut ToTable API'nı kullanabiliriz.
#endregion

#region Column - HasColumnName, HasColumnType, HasColumnOrder
/*
EF Core'da tabloların kolonları entity sınıfları içerisindeki property'lere karşılık gelmektedir. 
Default olarak property'lerin adı kolon adıyken, türleri/tipleri kolon türleridir.
Eğer ki generate edilecek kolon isimlerine ve türlerine müdahale etmek istiyorsak bu konfigürasyonu kullanırız.
*/
#endregion

#region ForeignKey - HasForeignKey
/*
İlişkisel tablo tasarımlarında, bağımlı tabloda esas tabloya karşılık gelecek verilerin tutulduğu kolonu Foreign Key olarak temsil etmekteyiz.
EF Core'da Foreign Key kolonu genellikle Entity Tanımlama Kuralları gereği default yapılanmalarla oluşturulur.
ForeignKey Data Annotations Attribute'unu direkt kullanabilirsiniz. 
Lakin Fluent API ile bu konfigürasyonu yapacaksak iki entity arasındaki ilişkiyide modellememiz gerekmektedir. 
Aksi taktirde Fluent API üzerinde HasForeignKey fonksiyonunu kullanamayız!
*/
#endregion

#region NotMapped - Ignore
/*
EF Core, entity sınıfları içerisindeki tüm property'leri default olarak modellenen tabloya kolon şeklinde migrate eder.
Bazen bizler entity sınıfları içerisinde tabloda bir kolona karşılık gelmeyen propertyler tanımlamak mecburiyetinde kalabiliriz.
Bu property'lerin EF Core tarafından kolon olarak map edilmesini istemediğimizi bildirebilmek için NotMapped ya da Ignore kullanabiliriz.
Böylece ilgili property sadece yazılımsal bir property olacaktır.
*/
#endregion

#region Key - HasKey
/*
EF Core'da, default convention olarak bir entity'nin içerisinde Id, ID, EntityId, EntityID vs. şeklinde tanımlanan tüm property'lere varsayılan olarak
primary key constraint uygulanır.
Key ya da HasKey yapılanmalarıyla istediğinmiz her hangi bir property'e default convention dışında Primary Key uygulayabiliriz.
EF Core'da bir entity içerisinde kesinlikle PK'i temsil edecek olan property bulunmalıdır. Aksi taktirde EF Core migration oluştururken hata verecektir.
Eğer ki tablonun PK'i yoksa bunun bildirilmesi gerekir. 
*/
#endregion

#region Timestamp - IsRowVersion
//İleride/sonraki derslerde veri tutarlılığı ile ilgili derste detaylı anlatılacak!
//Bu derste bir satırdaki verinin bütünsel olarak değişikliğini takip etmemizi sağlayacak olan versiyon mantığını konuşuyor olacağız.
//İşte bir verinin verisyonunu oluşturmamızı sağlayan yapılanma bu konfigürasyonlardır.
#endregion

#region Required - IsRequired
//Bir kolonun nullable ya da not null olup olmamasını bu konfigürasyonla belirleyebiliriz.
//EF Core'da bir property default oalrak not null şeklinde tanımlanır. Eğer ki property'si nullable yapmak istyorsak türü üzerinde ?(nullable) operatörü
//ile bildirimde bulunmamız gerekmektedir.
#endregion

#region MaxLenght | StringLength - HasMaxLength
//Bir kolonun max karakter sayısını belirlememizi sağlarlar.
#endregion

#region Precision - HasPrecision
//Küsüratlı sayılarda bir kesinlik belirtmemizi ve noktanın hanesini bildirmemizi sağlayan bir yapılandırmadır.
#endregion

#region Unicode - IsUnicode
//Kolon içerisinde unicode karakterler kullanılacaksa bu yapılandırmadan istifade edilebilir.
#endregion

#region Comment - HasComment
//EF Core üzerinden oluşturulmuş olan veritabanı nesneleri üzerinde bir açıklama/yorum istiyorsanız Comment'i kullanabilirsiniz.
#endregion

#region ConcurrencyCheck - IsConcurrencyToken
//İleride/sonraki derslerde veri tutarlılığı dersinde daha detaylı!
//Bu derste bir satırdaki verinin bütünsel olarak tutarlılığını sağlayacak bir Concurrency Token yapılanmasından bahsedilecektir.
#endregion

#region InverseProperty
//İki entity arasında birden fazla ilişki varsa eğer bu ilişkilerin hangi navigation property üzerinden olacağını ayarlamamızı sağlayan bir konfigürasyondur.
//Uçak ile Havaalanı arasındaki ilişkiyi değerlendirirsek uçağın bir adet kalktığı ve bir adet indiği havaalanı vardır. Bunun için uçak ve havaalanı arasında
//2 kere ilişki kurarız.

//Bu konunun FluentAPI kısmı ileride.
#endregion

#endregion

#region Configurations | Fluent API

#region Composite Key
//Tablolarda birden fazla kolonu kümülatif olarak primary key yapmak istiyorsak buna composite key denir.
#endregion

#region HasDefaultSchema
//EF Core üzerinden inşa edilen herhangi bir veritabanı nesnesi default olarak dbo şemasına sahiptir. Bunu özelleştirebilmek için kullanılan bir yapılandırmadır.
#endregion

#region Property

#region HasDefaultValue
//Tablodaki herhangi bir kolonun değer gönderilmediği durumlarda default olarak hangi değeri alacağını belirler.
#endregion

#region HasDefaultValueSql
//Default olarak alınacak değerin bir SQL komutunun çalışması ile vereceksek kullanılır.
//Tablodaki herhangi bir kolonun değer gönderilmediği durumlarda default olarak hangi SQL cümleciğinden değeri alacağını belirler.
#endregion

#endregion

#region HasComputedColumnSql
//Bir entity tasarlarken bu entity'nin içerisindeki birden fazla kolonu tek bir property'e yani kolona işlevsel olarak bağlayabiliriz.
//Ve bu kolon bağlı kolonlardaki verileri kullanarak bir sonuç üretebilir.
//Tablolarda birden fazla kolondaki veirleri işleyerek değerini oluşturan kolonlara Computed Column denmektedir.
//HasComputedColumnSql ise EF Core üzerinden bu tarz computed column oluşturabilmek için kullanılan bir yapılandırmadır.
#endregion

#region HasConstraintName
//EF Core üzerinden oluşturulan constraint'lere default isim yerine özelleştirilmiş bir isim verebilmek için kullanılan yapılandırmadır.
#endregion

#region HasData
//Sonraki derslerimizde Seed Data isimli bir konuyu incelenecek. Bu konuda migrate sürecinde veritabanını inşa ederken bir yandan da yazılım üzerinden hazır veriler oluşturmak
//istiyorsak eğer buunun yöntemini usulünü inceliyor olacağız.
//İşte HasData konfigürasyonu bu operasyonun yapılandırma ayağıdır.
//HasData ile migrate sürecinde oluşturulacak olan verilerin PK olan Id kolonlarına iradeli bir şekilde değerlerin girilmesi zorunludur!
#endregion

#region HasDiscriminator
//İleride entityler arasında kalıtımsal ilişkilerin olduğu TPT ve TPH isminde konularını inceliyor olacağız. İşte bu konularla ilgili yapılandırmalarımız
//HasDiscriminator ve HasValue fonksiyonlarıdır.

#region HasValue

#endregion

#endregion

#region HasField
//Backing Field özelliğini kullanmamızı sağlayan bir yapılandırmadır.
#endregion

#region HasNoKey
//Normal şartlarda EF Core'da tüm entitylerin bir PK kolonu olmak zorundadır. Eğer ki entity'de PK kolonu olmayacaksa bunun bildirilmesi gerekmektedir!
//İşte bunun için kullanılan fonksiyondur.
#endregion

#region HasIndex
//Sonraki derslerimizde EF Core üzerinden Index yapılanmasını detaylıca inceliyor olacağız.
//Bu yapılanmaya dair konfigürasyonlarımız HasIndex ve Index attribute'dur.
#endregion

#region HasQueryFilter
//İleride göreceğimiz Global Query Filter başlıklı dersin yapılandırmasıdır.
//Temeldeki görevi bir entity'e karşılık uygulama bazında global bir filtre koymaktır.
#endregion

#region DatabaseGenerated - ValueGeneratedOnAddOrUpdate, ValueGeneratedOnAdd, ValueGeneratedNever

#endregion
#endregion

//Main içindeyiz. RowVersion işlemlerini yapalım.
// Bir adet person ekleyelim.
/*
Person p = new();
p.Department = new()
{
    Name = "Yazılım Departmanı"
};
p.Name = "Ahmet";
p.Surname = "Daşkıran";

await context.Persons.AddAsync(p);
await context.SaveChangesAsync();
//Person'a baktığımızda RowVersion 209 görürüz.
*/
//var person = await context.Persons.FindAsync(1);
/*
person.Name = "Mehmet"; //eklediğimiz person'un adını değiştirelim.
await context.SaveChangesAsync();
*/
//Person'a tekrar baktığımızda RowVersion'un 210 olduğunu görürüz. Veri herhangi bir aksiyona uğradığında versiyonu artmıştır.

//-----

//Main İçerisinde HasDiscriminator işlemleri yapalım.
/*
A a = new()
{
    X = "A'dan",
    Y = 1
};

B b = new()
{
    X = "B'den",
    Z = 2
};

Entity entity = new()
{
    X = "Entity'den"
};
await context.As.AddAsync(a);
await context.Bs.AddAsync(b);
await context.Entities.AddAsync(entity);
await context.SaveChangesAsync();
//Buraya kadar olan işlemleri execute ettiğimizde tabloda Discriminator adlı kolonda A'dan gelen verileri A olarak, B'deb gelen verileri B olarak işaretlemiş,
//Entity'den gelen veriyi Entity olarak işaretlemiş olduğunu görürüz.
//Böyle bir çalışmada oluşturulan Discriminator kolonunun adına müdahele etmek istiyorsak FluentAPI'da HasDiscriminator'u kullanabiliriz.
*/

//------------
//[Table("Kisiler")] //Bu entity'e karşılık oluşturulacak tablonun adını Kisiler yaptık.
class Person
{

    //Net olarak görülüyor ki Id Primary Key.
    public int Id { get; set; }
    //[Key] //farklı bir isimdeki property'i PK yapmak istiyorsak [Key] eklememiz yeterli.
    //public int Sucuk { get; set; } //Artık Sucuk bir PK'dir.
    //public int Id2 { get; set; } //Composite Key yapmak istiyorsak Fluent API'da,

    //-----
    public int DepartmentId { get; set; } //ForeignKey'imiz.
    //farklı bir isimle ForeignKey olarak atamak istersek;
    [ForeignKey(nameof(Department))]
    public int Sebastian { get; set; } //Artık person'un içindeki Sebastian, Departments tablosundaki Id ile ilişkili.
    //-----

    //[Column("Adi", TypeName = "metin", Order = 7)] //türü metin name'i Adi olan bir kolon oluşturulacaktır. Data An. ile kolon isimlerine bu şekilde müdahele edilir.
    public string _name; //buradaki name'in bir backing field olduğunu bildirebilmek için HasField'ı kullanırız.
    public string Name { get; set; } //Order ile de kolonun kaçıncı sırada olacağını belirtiriz.
    //-----

    //[Required] //Surname'i not null olarak belirttik.
    public string? Surname { get; set; } //nullable yapabilmek için ise ? eklememiz gerekli. Required kullandıysak ?'in olmaması gerektiğini unutma.
    //-----

    [MaxLength(5)] // MySurname'nin maksimum kaç karakter olacağını belirttik.
    //[StringLength(15)]
    public string MySurname { get; set; }
    //-----

    [Unicode]
    public string YourSurname { get; set; }

    //-----

    [Precision(5,3)] //5 sayısal hane ve noktadan sonra 3 hane tut. (12.345 gibi) (12345 hata verir, noktadan sonra 3 hane girilmelidir.)
    public decimal Salary { get; set; }

    //Sadece yazılımsal amaçla bir property olsun bu. Fakat herhangi bir migration sürecinde yine de yakalanacaktır. Ve kolon olarak tabloya ekleyecektir.
    [NotMapped] //NotMapped'i koyduktan sonra EF Core'a eşleştirme yapma, bu bir kolona karşılık gelen property değil demiş oluyoruz.
    public string MuzluCikolata { get; set; } //Artık migration'da eklenmeyecektir. Tablomuzda gözükmeyecektir.

    //-----
    [Timestamp] //dediğimiz zaman aşağıdaki dizi versiyon mantığında çalışan bir yapılanmaya dönecektir.
    [Comment("Timestamp şu şu işlere yaramaktadır.")] 
    //bu açıklamayı yapıp migrate ettiğimizde SQL'de RowVersion tablosuna sağ tıklayıp Properties dediğimizde Extended Properties-Description kısmında ilgili açıklammayı görebiliriz.
    public byte[] RowVersion { get; set; } //Her bir Person satırına karşılık gelecek versiyonumuz.

    //-----
    [ConcurrencyCheck]
    public int ConcurrencyCheck { get; set; }
    //-----

    //eğer ki CreatedDate'e her seferinde SQL ile default bir değer vereceksek eğer;
    public DateTime CreatedDate { get; set; }
    public Department Department { get; set; }
}
class Department
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Person> Persons { get; set; }
}
class Example
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Computed { get; set; }
}
class Entity
{
    public int Id { get; set; }
    public string X { get; set; }
}
class A : Entity
{
    public int Y { get; set; }
}
class B : Entity
{
    public int Z { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Entity> Entities { get; set; }
    public DbSet<A> As { get; set; }
    public DbSet<B> Bs { get; set; }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Department> Departments { get; set; }

    //--- 
    //EF Core bu tip bir bizden fazla foreign key olacak yapılanmada hata verecektir. Bunu çözebilmek için InverseProperty kullanırız.
    //public DbSet<Flight> Flights { get; set; }
    //public DbSet<Airport> Airports { get; set; }

    //---
    public DbSet<Example> Examples { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region GetEntityTypes
        /*
        var entities = modelBuilder.Model.GetEntityTypes(); //uygulamada kullanılan tüm entity'leri getir. Mimaride diğer sınıflardan entity sınıflarını ayırmak
        //istersek GetEntityTypes kullanabiliriz.
        foreach (var entity in entities)
        {
            Console.WriteLine(entity.Name); //Şu anda DbContext'de kullandığımız tüm entity'leri ekrana yazdırdık.
        }
        */
        #endregion

        #region ToTable
        /*
        //Table Annotation kullanmayacaksak, FluentAPI ile bu işlemi gerçekleştirebiliriz.

        modelBuilder.Entity<Person>().ToTable("KisilerAmaFluent"); //Hangi entity'e müdahele edeceksek ona gidiyoruz ve oluşturulacak
        //tablonun ismini veriyoruz.

        //Aynı anda Data An. ve F.API kullanırsak; Fluent API geçerlidir.
        */
        #endregion

        #region Column
        /*
        modelBuilder.Entity<Person>() //önce ilgili entity'e gidelim
            .Property(p => p.Name) //sonra hangi kolonda çalışacaksak onun property'sine gidelim.
            .HasColumnName("Adi") //kolonların adın,
            .HasColumnType("mahmut") //türüne,
            .HasColumnOrder(7); //tablodaki sırasına müdahele edebiliriz.
        */
        #endregion

        #region ForeignKey
        /*
        //FluentAPI ile ForeignKey atamak istiyorsak önce iki tablo arasındaki ilişkiyi belirtmem gerekiyor.
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Department)
            .WithMany(d => d.Persons)
            .HasForeignKey(p => p.Sebastian); //FluentAPI ile Sebastian'ı ForeignKey olarak belirttik.
        */
        #endregion

        #region Ignore
        /*
        //Data Annotations'larda NotMapped ile yaptığımızı Ignore ile FluentAPI kullanarak yapabiliriz.
        modelBuilder.Entity<Person>() //Entity üzerinden Ignore'a gidip MuzluCikolata'yı Ignore edebiliriz.
            .Ignore(p => p.MuzluCikolata);

        //Notdip: Bazen Column'da olduğu gibi .Property'den bazen de burada olduğu gibi direkt Entity'den işlemleri yapmamız gerekir.
        */
        #endregion

        #region Primary Key
        //modelBuilder.Entity<Person>() //Entity üzerinden tablomuza gibip ilgili property'i PK atayabiliriz.
        //    .HasKey(p => p.Sucuk);
        #endregion

        #region IsRowVersion 
        //TimeStamp'ın işini FluentAPI ile yapmak için;

        //modelBuilder.Entity<Person>() //ilgili tabloya gidip,
        //    .Property(p => p.RowVersion) //Property'lere girip ilgili property'e,
        //    .IsRowVersion(); //IsRowVersion diyerek bu kolonun bir versiyonlama kolonu olduğunu belirtmiş oluyoruz.
        #endregion

        #region Required
        //FluentAPI ile de null operasyonlarını belirtebiliriz.
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Surname).IsRequired();
        #endregion

        #region MaxLength
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.MySurname)
        //    .HasMaxLength(5);
        #endregion

        #region Precision
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Salary)
        //    .HasPrecision(5, 3);
        #endregion

        #region Unicode
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.Surname)
        //    .IsUnicode();
        #endregion

        #region Comment
        //modelBuilder.Entity<Person>() //'e HasComment atarsak tablomuza yorum eklemiş oluruz.
        //        .HasComment("Bu tablo şuna yaramaktadır...")
        //    .Property(p => p.Surname) //ile de property'lere yani kolonlara yorum eklemiş oluruz.
        //        .HasComment("Bu kolon şuna yaramaktadır.");
        #endregion

        #region ConcurrencyCheck
        //modelBuilder.Entity<Person>()
        //    .Property(p => p.ConcurrencyCheck)
        //    .IsConcurrencyToken();
        #endregion

        #region CompositeKey
        //modelBuilder.Entity<Person>().HasKey("Id", "Id2"); //Yine HasKey'i kullanarak Id ve Id2'yi 2 tane Primary Key yani Composite Key olarak atayabiliriz.
        //modelBuilder.Entity<Person>().HasKey(p => new { p.Id, p.Id2 }); //Ya da özel tanımlı fonksiyonlarla anonim bir tür oluşturup bu tür üzerinden Id ve Id2'yi 
        //Composite key olarak atayabiliriz.
        #endregion

        #region HasDefaultSchema
        //modelBuilder.HasDefaultSchema("ahmet");//artık dbo. yerine ahmet. diye oluşturulacak.
        #endregion

        #region Property

        #region HasDefaultValue
        /*
        modelBuilder.Entity<Person>()
            .Property(p => p.Salary)
            .HasDefaultValue(100); //Salary'e default olarak 100 değerini ata. EF Core'da bir person eklediğimizde Salary'i girmesek dahi 100 olarak atanacaktır.
        */
        #endregion

        #region HasDefaultValueSql
        //Yazılım kısmından değil de SQL cümleciği üzerinden bir değer göndermek istiyorsak, yani execute esnasında veritabanında bir SQL cümleciği çalıştırılıp
        //ilgili kolon oradan değerini alacaksa HasDefaultValueSql'i kullanırız.
        /*
        modelBuilder.Entity<Person>()
            .Property(p => p.CreatedDate)
            .HasDefaultValueSql("GETDATE()"); //GETDATE ile o anın tarih bilgisini verir. 
        */
        #endregion

        #endregion

        #region HasComputedColumnSql
        /*
        modelBuilder.Entity<Example>()
            .Property(p => p.Computed)
            .HasComputedColumnSql("[X] + [Y]"); //SQL tablomuzda; X kolonuna 100, Y'ye 200 girersek Computed kolonu 300 değerini alacaktır.
        */
        #endregion

        #region HasConstraintName
        /*
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Department) //klasik bire çok ilişki belirlemesi.
            .WithMany(d => d.Persons)
            .HasForeignKey(p => p.DepartmentId) //foreign key'i belirttik. Bu şekilde bir migration oluşturduğumuzda FK_Persons_Departments_DepartmentId şeklinde default bir değer atar.
            .HasConstraintName("akrepnalan"); //bunu özelleştirmek istiyorsak eğer HasConstraintName'i kullanırız. Artık bu constraint'in adı akrepnalan'dır.
        */
        #endregion

        #region HasData
        /*
        modelBuilder.Entity<Department>().HasData(
            new Department()
            {
                Name = "Department 1",
                Id = 1
            });
        modelBuilder.Entity<Person>().HasData(
            new Person
            {
                Id = 1,
                DepartmentId = 1,
                Name = "turgut",
                Surname = "uyar",
                Salary = 100,
                CreatedDate = DateTime.Now
            },
            new Person
            {
                Id = 2,
                DepartmentId = 1,
                Name = "halide",
                Surname = "adıvar",
                Salary = 200,
                CreatedDate = DateTime.Now
            }
            );
        */
        #endregion

        #region HasDiscriminator
        /*
        //modelBuilder.Entity<Entity>() //Base Entity olan Entity'e gidiyoruz.
        //    .HasDiscriminator<string>("Ayirici"); //String olarak tutulup ismini Ayirici yapabiliriz.

        //ya da int olarak
        modelBuilder.Entity<Entity>() //Base Entity olan Entity'e gidiyoruz.
            .HasDiscriminator<int>("Ayirici")
            .HasValue<A>(1) //eğer ki A türünden bir veri geliyorsa int olarak 1 değerini ver.
            .HasValue<B>(2) //eğer ki B türünden bir veri geliyorsa int olarak 2 değerini ver.
            .HasValue<Entity>(3); //eğer ki Entity türünden bir veri geliyorsa int olarak 3 değerini ver.
        */
        #endregion

        #region HasField
        /*
        modelBuilder.Entity<Person>()
            .Property(p => p.Name)
            .HasField(nameof(Person._name));
        */
        #endregion

        #region HasNoKey
        //modelBuilder.Entity<Example>()
        //    .HasNoKey();
        #endregion

        #region HasIndex
        //modelBuilder.Entity<Person>()
        //    .HasIndex(p => new { p.Name, p.Surname });
        #endregion

        #region HasQueryFilter
        //modelBuilder.Entity<Person>()
        //    .HasQueryFilter(p => p.CreatedDate.Year == DateTime.Now.Year); 
        //sadece bu yıl eklenmiş olan personeller hangisiyse default olarak onların üzerinden bilgi getir demiş oluyoruz.
        //bu ayarlamayı yaptıktan sonra context.Persons.ToList() dediğimizde ekstradan gizli olarak .Where(p => p.CreatedDate.Year...'ı da eklemiş olacaktır.
        #endregion
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}

public class Flight //dependent entity
{
    public int FlightID { get; set; }
    public int DepartureAirportId { get; set; }
    public int ArrivalAirportId { get; set; }
    public string Name { get; set; }
    public Airport DepartureAirport { get; set; } //Her uçuşun bir kalkış noktası var.
    public Airport ArrivalAirport { get; set; } //Bir de varış havalimanı var.
}

public class Airport //principle entity
{
    public int AirportID { get; set; }
    public string Name { get; set; }

    [InverseProperty(nameof(Flight.DepartureAirport))] //alttaki satırdaki ICollection'a bağlantının nereden geleceğini dependent entity'deki DepartureAirport ile belirtiyoruz.
    public virtual ICollection<Flight> DepartingFlights { get; set; }

    [InverseProperty(nameof(Flight.ArrivalAirport))] //aynı şekilde bu sefer ArrivalAirport'u veriyoruz.
    public virtual ICollection<Flight> ArrivingFlights { get; set; }
}

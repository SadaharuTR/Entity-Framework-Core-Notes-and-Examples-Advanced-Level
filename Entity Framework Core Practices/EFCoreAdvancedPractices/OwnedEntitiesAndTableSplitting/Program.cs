using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Owned Entity Types Nedir?
//EF Core Entity sınıflarını parçalayarak, Property'lerini kümesel olarak farklı sınıflarda barındırmamıza ve tüm bu sınıfları ilgili entity'de birleştirip
//bütünsel olarak çalışmamıza izin vermektedir.

//Böylece bir Entity, sahip olunan(Owned) birden fazla alt sınıfın birleşmesiyle meydana gelebilmektedir.
#endregion

#region Owned Entity Types'ı Neden Kullanırız?
//https://www.gencayyildiz.com/blog/wp-content/uploads/2020/12/Entity-Framework-Core-Owned-Entities-and-Table-Splitting.png
//Blog linki: https://www.gencayyildiz.com/blog/entity-framework-core-owned-entities-and-table-splitting/

//Domain Driven Design(DDD) yaklaşımında Value Object'lere karşılık olarak Owned Entity Types'lar kullanılır!
//Detaylı: https://www.gencayyildiz.com/blog/domain-driven-design-stratejik-ve-taktiksel-olarak-derinlemesine-inceleme/
#endregion

#region Owned Entity Types Nasıl Uygulanır?
//Normal bir entity'de farklı sınıfların referans edilmesi Primary Key gibi hatalara sebebiyet verecektir. Çünkü direkt bir sınfın referans olarak alınması
//EF Core tarafından ilişkisel bir tasarım olarak algılanır. Bizlerin Entity ieçrisindeki propertyleri kümesel olarak barındıran sınıfları o entity'nin bir parçası
//olduğunu bildirmemiz özellikle gerekmektedir. Bunları aşağıdaki 3 yöntemle bildiririz.

#region OwnsOne Metodu 

#endregion

#region Owned Attribute'u
//ile ilgili sınıfları işaretlersek onların gerçek bir entity değil owned enetity türleriolduğunu belirtmiş oluruz.
#endregion

#region IEntityTypeConfiguration<T> Arayüzü
//Entity Framework Core #33 - IEntityTypeConfiguration İle Yapılandırmaları Harici Dosyalara Ayırmak
#endregion

#region OwnsMany Metodu
//OwnsMany metodu, Entity'nin farklı özelliklerine başka bir sınıftan, ICollection türünde, Navigation Property aracılığıyla ilişkisel olarak erişebilmemizi sağlayan bir işleve sahiptir.
//Normalde Has ilişki olarak kurulabilecek bu ilişkinin temel farkı, Has ilişkisi DbSet property'si gerektirirken, OwnsMany metodu ise DbSet'e ihtiyaç duymaksızın gerçekleştirmemizi
//sağlamaktadır.

//Artık Employee üzerinden Order'ları sorgulayabilecek, fakat Order'lara direkt erişim gösteremeyeceğiz. Çünkü Order'lar DbSet'e karşılık gelen bir Entity olmayacak.

//var d = await context.Employees.ToListAsync(); //Sorguladığımızda d içerisinde Order'ları da Owned Entity olarak göreceğiz.
//Console.WriteLine();

#endregion
#endregion

#region İç İçe Owned Entity Types

#endregion

#region Sınırlılıklar
//Herhangi bir Owned Entity Type için DbSet Property'sine ihtiyaç yoktur.

//OnModelCreating fonksiyonunda direkt Entity<T> metodu ile Owned Entity Type türünden bir sınıf üzerinde herhangi bir konfigürasyon gerçekleştirilemez.
//OwnsOne ya da OwnsMany'den hangisini kullanıyorsak orada bu konf. gerçekleştirebiliriz.

//Owned Entity Type'ların kalıtımsal hiyerarşi desteği yoktur.
#endregion

class Employee
{
    public int Id { get; set; }
    //public string Name { get; set; }
    //public string MiddleName { get; set; }
    //public string LastName { get; set; }
    //public string StreetAddress { get; set; }
    //public string Location { get; set; }
    public bool IsActive { get; set; }

    public EmployeeName EmployeeName { get; set; }
    public Address Adress { get; set; }

    public ICollection<Order> Orders { get; set; } //Principle Entity'de Order'ı yine de referans etmemiz lazım.
}
class Order //Order'ın Owned Entity olmasını istiyorsak PK'siz ve NP'siz tasarlamamız lazım.
{
    public string OrderDate { get; set; }
    public int Price { get; set; }
}
//[Owned] //Attribute'u ile işaretleyebiliriz.
class EmployeeName
{
    public string Name { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public EmployeBilmemneName EmployeBilmemneName { get; set; }
}

class EmployeBilmemneName
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}
//[Owned] //Attribute'u ile işaretleyebiliriz.
class Address
{
    public string StreetAddress { get; set; }
    public string Location { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region OwnsOne
        /*
        modelBuilder.Entity<Employee>().OwnsOne(e => e.EmployeeName, builder =>
        {
            builder.Property(e => e.Name).HasColumnName("Name"); //default olarak oluşturulan kolon ismine bu şekilde müdahele edebiliriz. EmployeeName_Name yerine sadece Name olacaktır.
        });
        modelBuilder.Entity<Employee>().OwnsOne(e => e.Adress);
        */
        #endregion

        #region OwnsMany
        modelBuilder.Entity<Employee>().OwnsMany(e => e.Orders, builder =>
        {
            builder.WithOwner().HasForeignKey("OwnedEmployeeId");
            builder.Property<int>("Id");
            builder.HasKey("Id");
        });
        #endregion

        //EmployeeConfiguration : IEntityTypeConfiguration<Employee> 'ı bildirmemiz gerekli.
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    }
    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server = PC\\SQLEXPRESS; Database = ApplicationDb; User ID = SA; Password = 1; TrustServerCertificate = True");
    }
}

//IEntityTypeConfiguration<T> Arayüzü ile;
class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.OwnsOne(e => e.EmployeeName, builder =>
        {
            builder.Property(e => e.Name).HasColumnName("Name");
        });
        builder.OwnsOne(e => e.Adress);
    }
}
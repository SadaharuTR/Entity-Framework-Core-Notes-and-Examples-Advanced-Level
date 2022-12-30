using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Reflection.Emit;

ApplicationDbContext context = new();

//EF Core'da SP ve View'ler için hazır bir yapılanma yoktur.

#region Stored Procedure Nedir?
//SP, View'ler gibi kompleks sorgularımızı daha basit bir şekilde tekrar kullanılabilir
//bir hale getirmemizi sağlayan veritabanı nesnesidir. 
//View'ler tablo misali bir davranış sergilerken, SP'lar ise fonksiyonel bir davranış sergilerler.
//Ve türlü türlü artıları da vardır.
#endregion

#region EF Core İle Stored Procedure Kullanımı

#region Stored Procedure Oluşturma
//1. adım : Boş bir migration oluşturunuz.
//2. adım : Migration'ın içerisindeki Up fonksiyonuna SP'ın Create komutlarını yazınız,
//Down fonksşyonuna ise Drop komutlarını yazınız.
//3. adım : Migrate ediniz.
#endregion

#region Stored Procedure'ü Kullanma
//SP'ı kullanabilmek için bir Entity'e ihtiyacımız vardır. Bu Entity'nin DbSet property'si olarak context
//nesnesine de eklenmesi gerekmektedir.
//Bu DbSet properyty'si üzerinden FromSql fonksiyonunu kullanarak 'Exec ....' komutu eşliğinde SP
//yapılanmasını çalıştırıp sorgu neticesini elde edebilirsiniz.

#region FromSql
//EXEC ile SP'i execute ederiz. FromSql, DbSet property'leri üzerinden erişilebilir.
/*
var datas = await context.PersonOrders.FromSql($"EXEC sp_PersonOrders")
    .ToListAsync();
*/
#endregion
#endregion

#region Geriye Değer Döndüren Stored Procedure'ü Kullanma
//En çok satış yapan elemanın satış değerini geriye döndürme çalışması yapabiliriz.

//EXEC işleminden önce @count'u bir şekilde oluşturmuş olmamız gerekli.
/*
SqlParameter countParameter = new()
{
    ParameterName = "count", //adı count olsun. @count ismi ne ise onunla uyumlu olmalı.
    SqlDbType = System.Data.SqlDbType.Int, //parametrenin türünü belirtelim.
    Direction = System.Data.ParameterDirection.Output 
    //@count'a gelecek olan veriyi, SP'un return'unden elde edeceğimizden dolayı Output olarak bildirelim.
};

//Veritabanı üzerinden bir sorgulama işlemi gerçekleştireceğiz. Bunu yaparken,
//Herhangi bir türe bağlı olmadığımızdan dolayı Database property'sinden istifade edeceğiz.
//Herhangi bir entity ile işimiz yok çünkü yaptığımız çalışma bize tür değil değer döndürüyor.
//SP'mizin geri dönüş değerini @count'a assign ediyoruz.
await context.Database.ExecuteSqlRawAsync($"EXEC @count = sp_BestSellingStaff", countParameter);
//ne zamanki üst satırdaki kod execute edilir, o zaman countParameter isimlii nesnemizin value property'si
//doldurulur.
Console.WriteLine(countParameter.Value);
*/
#endregion

#region Parametreli Stored Procedure Kullanımı
#region Input Parametreli Stored Procedure'ü Kullanma
#endregion
#region Output Parametreli Stored Procedure'ü Kullanma
#endregion

SqlParameter nameParameter = new()
{
    ParameterName = "name",
    SqlDbType = System.Data.SqlDbType.NVarChar,
    Direction = System.Data.ParameterDirection.Output,
    Size = 1000
};
//sp_PersonOrderWithParameters 7 ile input'u @name ile OUTPUT'u veriyoruz.
await context.Database.ExecuteSqlRawAsync($"EXECUTE sp_PersonOrderWithParameters 7, @name OUTPUT", nameParameter);
Console.WriteLine(nameParameter.Value);

#endregion
#endregion
Console.WriteLine();
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

    public Person Person { get; set; }
}
[NotMapped] //ile PersonOrder'ı işaretlemezsek migration sürecinde tablo olarak oluşturacaktır.
public class PersonOrder 
//SP çalışması neticesinde gelecek olan tabloyu modelleyeceğimiz bir Entity oluşturalım. 
{
    public string Name { get; set; }
    public int Count { get; set; }
}
class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<PersonOrder> PersonOrders { get; set; } //DbSet'imizi de oluşturalım.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        //PK olmadığını bildirmemiz lazım. SP ile ilgili bir bildirimde bulunamıyoruz.
        modelBuilder.Entity<PersonOrder>()
            .HasNoKey();

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
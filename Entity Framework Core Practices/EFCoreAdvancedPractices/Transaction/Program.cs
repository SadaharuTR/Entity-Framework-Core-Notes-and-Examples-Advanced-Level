
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Transactions;

ApplicationDbContext context = new();
#region Transaction Nedir?
//Bir veritabanı üzerinde işlem yaparken bu işlemin kesinlikle bir Transaction üzerinde gerçekleştirilmesi gerekmektedir.
//Transaction, veritabanındaki kümülatif işlemleri atomik bir şekilde gerçekleştirmemizi sağlayan bir özelliktir.

//Bir transaction içerisindeki tüm işlemler commit edildiği taktirde veritabanına fiziksel olarak yansıtılacaktır.
//Ya da rollback edilirse tüm işlemler geri alınacak ve fiziksel olarak veritabanında herhangi bir verisel değişiklik durumu söz konusu olmayacaktır.

//Transaction'ın genel amacı veritabanındaki tutarlılık durumunu korumaktadır. Ya da bir başka deyişle veritabanındaki tutarsızlık durumlarına
//karşı önlem almaktır.
#endregion

#region Default Transaction Davranışı
//EF Core'da varsayılan olarak, yapılan tüm işlemler SaveChanges fonksiyonuyla veritabanına fiziksel olarak uygulanır. 
//Çünkü SaveChanges default olarak bir transaction sahiptir.

//Eğer ki bu süreçte bir problem/hata/başarısızlık durumu söz konusu olursa tüm işlemler geri alınır(rollback) ve işlemlerin hiçbiri veritabanına
//uygulanmaz.

//Böylece SaveChanges tüm işlemlerin ya tamamen başarılı olacağını ya da bir hata oluşursa veritabanını değiştirmeden işlemleri sonlandıracağını
//ifade etmektedir.
#endregion

#region Transaction Kontrolünü Manuel Sağlama
/*
IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
//EF Core'da transaction kontrolü iradeli bir şekilde manuel sağlamak yani elde etmek istiyorsak eğer BeginTransactionAsync fonksiyonu çağrılmalıdır.
//Default'dan çıktık. Artık commit etmedikçe değişiklik veritabanına yansımayacak.
Person p = new() { Name = "Arnold2" };
await context.Persons.AddAsync(p);
await context.SaveChangesAsync();

await transaction.CommitAsync();
*/
//Transaction'u commit etmezsek default olarak Rollback anlamına gelecektir. Manuel olarak Rollback için RollbackAsync()'i çağırabiliriz.
#endregion

#region Savepoints
//EF Core 5.0 sürümüyle gelmiştir.
//Savepoints, veritabanı işlemleri sürecinde bir hata oluşursa veya başka bir nedenle yapılan işlemlerin geri alınması gerekiyorsa transaction
//içerisinde dönüş yapılabilecek noktaları ifade eden bir özelliktir.

#region CreateSavepoint
//Transaction içerisinde geri dönüş noktası oluşturmamızı sağlayan bir fonksiyondur.
#endregion

#region RollbackToSavepoint
//Transaction içerisinde herhangi bir geri dönüş noktasına(Savepoint'e) rollback yapmamızı sağlayan fonksiyondur.
#endregion

//Savepoints özelliği bir transaction içerisinde istenildiği kadar kullanılabilir.
/*
IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(); //manuel olarak transaction'ı ele alalım.

Person p13 = await context.Persons.FindAsync(13); //11 ve 13. Id'lerdeki Person'ları elde edelim.
Person p11 = await context.Persons.FindAsync(11);
context.Persons.RemoveRange(p13, p11);
await context.SaveChangesAsync();

await transaction.CreateSavepointAsync("t1"); //11 ve 13'ü sildiğimiz noktaya Rollback noktası açalım. Adını t1 koyalım.

Person p10 = await context.Persons.FindAsync(10); //10 Id'li Person'ı elde etme ve silme işlemini yapalım.
context.Persons.Remove(p10);
await context.SaveChangesAsync();

//Dipnot: RollbackAsync yapılan tüm işlemleri geri alır.
await transaction.RollbackToSavepointAsync("t1"); //bu noktada transaction'u save noktasına rollback edelim.
//t1'den öncesi fiizksel olarak veritabanına işlenmiş olacak fakat sonrası işlenmeyecektir. 10 Id'li şahıs silinmeyecektir.
await transaction.CommitAsync();
*/
#endregion

#region TransactionScope  
//Veritabanı işlemlerini bir grup olarak yapmamızı sağlayan bir sınıftır.
//ADO.NET ile de kullanılabilir.

using TransactionScope transactionScope = new(); //önce hazır TransactionScope sınıfından bir nesne üretilir.
//Veritabanı işlemleri...
//..
//.. gerçekleştirilir.
transactionScope.Complete(); //Complete fonksiyonu yapılan veritabanı işlemlerinin commit edilmesini sağlar.
//Eğer ki rollback yapacaksanız complete fonksiyonunun tetiklenmemesi yeterlidir! using bloğu ne zaman dispose ederse yapılan tüm veritabanı işlemleri
//rollback edilir.

#region Complete

#endregion
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
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System;
using System.Collections.Generic;

ApplicationDbContext context = new();

/*
Kütüphane Kurulumu
Entity Framework Core ile in-memory’de çalışabilmek için ilgili projeye 
Microsoft.EntityFrameworkCore kütüphanesiyle birlikte 
Microsoft.EntityFrameworkCore.InMemory kütüphanesinin yüklenmesi gerekmektedir. 
*/
//In-Memory database üzerinde çalışırken migration oluşturmaya ve migrate etmeye gerek yoktur!
//In-Memory'de oluşturulmuş olan database uygulama sona erdiği/kapatıldığı taktirde bellekten silinecektir.
//Dolayısıyla özellikle gerçek uygulamalarda in-memory database'i kullanıyorsanız bunun kalıcı değil geçici yani silinebilir bir özellik olduğunu
//UNUTMA!

#region EF Core'da In-Memory Database ile Çalışmanın Gereği Nedir?
//Genellikle bu özelliği yeni çıkan EF Core özelliklerini test edebilmek için kullanabiliriz. (Bkz. Gençay sensei)
//EF Core, fiziksel veritabanlarından ziyade in-memory'de Database oluşturup üzerinde birçok işlemi yapmamızı sağlayabilmektedir.
//İşte bu özellik ile gerçek uygulamaların dışında test gibi operasyonları hızlıca yürütebileceğimiz imkanlar elde edebilmekteyiz.
#endregion

#region Avantajları Nelerdir?
//Test ve pre-prod(daha yayına çıkmadan, pre production) uygulamalarda gerçek/fiziksel veritabanları oluşturmak ve yapılandırmak yerine tüm veritabanını
//bellekte modelleyebilir ve gerekli işlemleri sanki gerçek bir veritabanında çalışıyor gibi orada gerçekleştirebiliriz.

//Bellekte çalışmak geçici bir deneyim olacağı için veritabanı serverlarında test amaçlı üretilmiş olan veritabanlarının lüzumsuz yer işgal
//etmesini engellemiş olacaktır.

//Bellekte veritabanını modellemek kodun hızlı bir şekilde test edilmesini sağlayacaktır.
#endregion

#region Dezavantajları Nelerdir?
//In-Memory'de yapılacak olan veritabanı işlevlerinde ilişkisel modellemeler YAPILAMAMAKTADIR!
//Bu durumdan dolayı veri tutarlılığı sekteye uğrayabilir ve istatiksel açıdan yanlış sonuçlar elde edilebilir.
#endregion

#region Örnek Çalışma

await context.Persons.AddAsync(new() { Name = "Gabriel", Surname = "Batistuta" });
await context.SaveChangesAsync(); //Sanki gerçek veritabanında çalışıyormuş gibi execute işleminin yapılması gerekmektedir.

//Constraint'ler kullanılamamaktadır.

var persons = await context.Persons.ToListAsync();
Console.WriteLine();
#endregion

class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("exampleDatabase"); //In Memory'de exampleDatabase'i yoksa oluşturacaktır. Varsa kullanacaktır.
    }
}
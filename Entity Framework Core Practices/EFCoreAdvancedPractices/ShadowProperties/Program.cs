using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

ApplicationDbContext context = new();

#region Shadow Properties - Gölge Özellikler
//Entity sınıflarında fiziksel olarak tanımlanmayan/modellenmeyen ancak EF Core tarafından ilgili entity için var olan/var olduğu kabul edilen property'lerdir.
//Tabloda görülmesini istemediğimiz/lüzumlu görmediğimiz/entity instance'ı üzerinde işlem yapmayacağımız kolonlar için Shadow Property'ler kullanabiliriz.
//Shadow property'lerin değerleri ve state'leri de Change Tracker tarafından kontrol edilir.
#endregion

#region Foreign Key - Shadow Properties
//İlişkisel senaryolarda foreign key property'sini tanımlamadığımız halde EF Core tarafından Dependent Entity'e eklenmektedir. İşte bu Shadow Property'dir.
/*
var blogs = await context.Blogs.Include(b => b.Posts)
    .ToListAsync();
*/
#endregion

//İlişkisel senaryoların dışında herhangi bir entity'nin üzerinde shadow property oluşturabiliriz.

#region Shadow Property Oluşturma
//Bunu yapabilmek için Fluent API'yı kullanmanız gerekmektedir.
/* OnModelCreating'i kurcalamamız lazım.
modelBuilder.Entity<Blog>()
    .Property<DateTime>("CreatedDate"); 'ı ekleyelim. Aşağıda detaylı.
*/
#endregion

#region Shadow Property'e Erişim Sağlama
#region ChangeTracker İle Erişim
/*
//Shadow Property'e erişim sağlayabilmek için Change Tracker'dan istifade edilebilir.

var blog = await context.Blogs.FirstAsync(); //ilk Blog'umuzu elde edelim.

var createDate = context.Entry(blog).Property("CreatedDate"); 
//Entry üzerinden CT kullanacağız. Entry'nin içerisine elde ettiğimiz blog'u verelim.
//.Property() ile (oluştururken de erişim gösterecekken de) erişim gösteriyoruz. İçindeki CreatedDate ismindeki property üzerinde artık işlem yapabiliriz.

Console.WriteLine(createDate.CurrentValue); //şu andaki mevcut değerini çağırıp okuyabiliriz.
Console.WriteLine(createDate.OriginalValue); //ilgili CreatedDate'deki değeri InMemory'de değiştirdiysek CurrentValue ona göre şekillenir.
//Bunu SaveChanges ile veritabanına yansıtmadığımız sürece OriginalValues'u olarak kalacaktır.

//eğer ki değerlere müdahele etmek istiyorsak;

createDate.CurrentValue = DateTime.Now; //şu anki zaman ile CreatedDate kolonunu güncellemiş oluyoruz.
await context.SaveChangesAsync(); //bu değişikliği de veritabanına bildirdik.
*/
#endregion

#region EF.Property İle Erişim

//Özellikle LINQ sorgularında Shadow Propery'lerine erişim için EF.Property static yapılanmasını kullanabiliriz.
//örneğin, oluşturulma sıralamasına göre büyükten küçüğe (ya da tersi) göre buradaki blogları elde etmek istersek; (veya benzeri where sorguları oluşturmak)

var blogs = await context.Blogs.OrderBy(b => EF.Property<DateTime>(b, "CreatedDate")).ToListAsync();
//EF.Property metodu üzerinden şu anda üzerinde çalışacağımız property'nin türünü generic DateTime diyip (tip güvenliği için) CreatedDate Shadow Property'sinin
//verisini kullanarak OrderBy'la tarihi küçükten büyüğe sırala.

//Yılı 2020'den büyük olan tarihlere sahip Blog'ları elde et.
var blogs2 = await context.Blogs.Where(b => EF.Property<DateTime>(b, "CreatedDate").Year > 2020).ToListAsync();

//Tarihsel işlem yaptığımız durumlarda, son güncelleme tarihi gibi kolonlar tutuluyorsa; bunları entity'ler üzerinden yönetmektense entity'leri bu şekilde
//property'ler ile doldurmaktansa Shadow Property'leri kullanabiliriz. Developer açısından göz kirliliği olmasın, kafaları karışmasın.
#endregion
#endregion

class Blog
{
    public int Id { get; set; }
    public string Name { get; set; }

    //Bu blogla ilgili oluşturulma tarihini SHadow Property ile tutmak isteyebiliriz. OnModel'da <DateTime>. Blog'un içerisinde ekstradan bir property eklemiş
    //olmasak dahi OnModelCreating'de yaptığımız işlem neticesinde yeni bir migration oluşturduğumuzda EF Core ilgili Blog'a karşılık gelecek tabloda
    //yeni bir CreatedDate isminde kolon eklemiş olacaktır. Gölgelenmiş...
    public ICollection<Post> Posts { get; set; }
}

class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool lastUpdated { get; set; }

    public Blog Blog { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //EF Core herhangi bir entity için shadow property oluşturacaksak;
        modelBuilder.Entity<Blog>() //hangi entity üzerinde işlem yapacaksak ona gidelim. 
            //.Property<DateTime>(p => p.Name); ile var olan Propertylerden herhangi birini bu şekilde konfigüre edebiliriz.
            //Shadow Property oluşturacaksak generic yapılanmalı olanı tercih etmeliyiz. Bu şekilde oluşturacağımız Shadow Property'nin türünü generic olarak bildirip, devam edebiliriz.
            .Property<DateTime>("CreatedDate"); //.Property ile Blog'un içerisindeki property'ler ile ilgili bir işlem yapacağım anlamına gelmektedir.
            //DateTime türünde adı CreatedDate olan bir property tanımla dediğimiz an Shadow Property'miz hazır.
        base.OnModelCreating(modelBuilder);
    }
}
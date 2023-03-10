using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security;

Console.WriteLine();

//Seed Data'lar migration'ların dışında eklenmesi ve değiştirilmesi beklenmeyen durumlar için kullanılan bir özelliktir.

#region Data Seeding Nedir?
//EF Core ile inşa edilen veritabanı içerisinde veritabanı nesneleri olabileceği gibi verilerin de migrate sürecinde üretilmesini isteyebiliriz. (mesela Test için)
//işte bu ihtiyaca istinaden Seed Data Özelliği ile EF Core üzerinden migration'larda veriler oluştururabilir ve migrate ederken bu verileri hedef tablolarımıza basabiliriz.
//Seed Data'lar, migrate süreçlerinde hazır verileri tablolara basabilmek için bunları yazılım kısmında tutmamızı gerektirmektedirler. Böylece bu veriler üzerinde veritabanı
//seviyesinde istenilen manipülasyonlar rahatlıkla gerçekleştirilebilmektedir.

//Data Seeding özelliği;
//Test için geçici verilere ihtiyaç varsa,
//Asp.NET Core'daki Identity yapılanmasındaki roller gibi static değerler de tutulabilir.
//Yazılım için temel konf. değerler olabilir.
#endregion

#region Seed Data Ekleme

//OnModelCreating metodu içerisinde Entity fonksiyonundan sonra çağrılan HasData fonksiyonu, ilgili entity'e karşılık Seed Data'ları eklememizi sağlayan bir fonksiyondur.
//PK değerlerinin manuel olarak bildirilmesi/verilmesi gerekmektedir! Çünkü ilişkisel verileri de Seed Datalarla üretebilmemiz gerekmekte. PK değerini başta bilerek eklemek,
//veritabanına git-gel yapıp değeri kontrol etmekten daha az uğraştırıcıdır.
#endregion

#region İlişkisel Tablolar İçin Seed Data Ekleme

//İlişkisel senaryolarda Dependent Table'a veri eklerken Foreign Key kolonunun property'si varsa eğer ona ilişkisel değerini vererek ekleme işlemini yapıyoruz.
//Foreign Key yoksa Seed Data ekleyemiyoruz.

#endregion

#region Seed Datanın Primary Key'ini Değiştirme

//Eğer ki migrate edilen herhangi bir seed datanın sonrasında PK'sı değiştirilirse bu datayla ilişkisel başka veriler varsa onlara cascade davranışı sergilenecektir.

//Seed Data'lar bir kere migrate edilirler. Bir kere migrate edildikleri için (mig_1) sonraki migration'larda bunların insert edilmesi ilgili herhangi bir ibare olmayacağından
//ve mig_2'de direkt BlogId üzerinden bir değişiklik yapıldığına dair ibaremiz olacağından dolayı BlogId=1 ile ilgili işlemleri önceki migration'da tamamlıyoruz,
//ikinci migration devreye girerken new Post() { Id = 1, BlogId = 1,... ve new Post() { Id = 2, BlogId = 1,.. cascade ile silineceğinden dolayı da bir tutarsızlık meydana gelmez.

//eğer ki tüm migration'ları silip aşağıdaki haliyle baştan mig_1 oluşturursak patlarız.
#endregion

class Post
{
    public int Id { get; set; }
    public int BlogId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public Blog Blog { get; set; }
}

class Blog
{
    public int Id { get; set; }
    public string Url { get; set; }
    public ICollection<Post> Posts { get; set; }
}

class ApplicationDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>() //İlişkisel tablolarda ekleme işlemi.
            .HasData(
                new Post() { Id = 1, BlogId = 1, Title = "A", Content = "..." }, //A ve B BlogId'si 1 olana gitsin.
                new Post() { Id = 2, BlogId = 1, Title = "B", Content = "..." },
                new Post() { Id = 3, BlogId = 2, Title = "B", Content = "..." }
            //new Post() { Id = 5, BlogId = 3, Title = "B", Content = "..." } //olmayan bir BlogId'ye karşılık Post üretmeye çalışırsak migrate ederken hata alırız.
            );

        modelBuilder.Entity<Blog>() //veritabanını migrate ederken, Blog entity'sine karşılık tabloya
            .HasData(
                new Blog() { Id = 1, Url = "www.kalempil.com/blog" }, //şu veriyi ekleyerek migrate et.
                //new Blog() { Id = 11, Url = "www.kalempil.com/blog" }, mesela Id'si 1 i yanlış yazdık 11 olması gerekli. 1'in yerine 11 yazıp tekrar migrate ettiğimizde
                //değişiklik yansıtılacaktır.
                new Blog() { Id = 2, Url = "www.senibenibizi.com/blog" }
            );
        //Eğer ki yapılan değişikliği veritabanına direkt yansıtırsak buradaki davranış EF Core tarafından Cascade davranışı olacaktır. Yani önceki veri silinirken bu veriyle
        //ilişkisel olan veriler silinecek ardından yeni değer yani Id=11 eklenecektir. Yani ilişkisel veriler gg.


        //Migrate ettikten sonra tabloya manuel veri eklersek Id'nin PK'ine göre varsa identity özelliği devamını getirip 1-1 artacaktır.
        //Seed Data'lar tablonun yapısını bozmaz.
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
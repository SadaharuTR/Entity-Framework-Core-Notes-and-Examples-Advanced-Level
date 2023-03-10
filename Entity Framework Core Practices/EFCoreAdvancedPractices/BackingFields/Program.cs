using Microsoft.EntityFrameworkCore;

BackingFieldDbContext context = new();

var person = await context.Persons.FindAsync(1); //1 Id'sine karşılık gelen Person'ı sorguluyalım. name kısmına bakarsak null olarak gelecektir.
//çünkü name field'ının herhangi bir eşleştirmesini yapmadık. Entity içerisinde olan her field, bir backing field değildir. Bunu bildirmek lazım.

//Person person2 = new() //kayıt işlemi gerçekleştirirsek de bu kapsülleme işlemi geçerlidir. Kod çalışıp veri tabanına veriyi ekleyecektir.
//{
//    Name = "Person 1892", //set durumundaki kapsülleme sonrası eklersek substring(0,3)'ten dolayı tabloda Name'de Per olarak kapsülleyip eklemiş olacağız.
//    Department = "Department Alican"
//};

//1892 Şanlı Liverpool'umuzun kuruluş tarihidir.

//await context.Persons.AddAsync(person2);
//await context.SaveChangesAsync();

Console.Read();

#region Backing Fields
//Tablo içerisindeki kolonları entity class'ları içerisinde property'ler ile değil field'larla temsil etmemizi sağlayan bir özelliktir.
/*
class Person
{
    public int Id { get; set; }

    public string name;
    
    //public string Name { get => name; set => name = value } 
    //artık yapılan sorgu neticesinde name field'ı devreye girmiştir.
    //Eğer ki biz backing field kullanırsak, veritabanıyla olan iletişim etkileşim sürecinde bir encapsulation gerçekleştirebiliriz demektir.
    //yani veritabanından gelen veriyi kapsülleyerek dış dünyaya açabilir, aynı şekilde veritabanına göndereceğimiz verilerin üzeirnde de
    //ekstradan farklı bir kapsülleme gerçekleştirip veritabanına gönderebiliriz.
    
    //public string Name { get => name.Substring(0, 3); set => name = value; } //artık gelen datayı istediğimiz gibi manipüle edebiliriz.
    public string Name { get => name.Substring(0, 3); set => name = value.Substring(0,3); } //set'i de manipüle edebiliriz.
    public string Department { get; set; }
}
*/
#endregion

#region BackingField Attributes
/*
//bu şekilde uyguladığımızda artık sorgulama neticesinde gelecek verilerin sadece field kullanarak işlendiğini gözlemleriz.
//backing field attribute'unu kullaranarak bir çalışma yapıp, üzerine sorgulama gerçekleştirirsek sadece field'ı kullanmış oluruz.
class Person
{
    public int Id { get; set; }
    public string name;
    [BackingField(nameof(name))] //Backing Field Attribute'u üzerinden name field'ını bildirmemiz lazım.
    //herhangi bir member'ın ismini string olarak bir yere yazacaksak namof operatörünü kullanıyoruz.
    public string Name { get; set; } //bu property'e gelecek olan veriler name ismini taşıyan field'a gönderilmiş olsun dedik.
    public string Department { get; set; }
}
*/
#endregion

#region HasField Fluent API
//Fluent API'da HasField metodu BackingField özelliğine karşılık gelmektedir.
//Bu şekilde bir FindAsync ile sorgulama yaptığımızda var person içerisinde property yerine field'ımızın kullanıldığını görürüz.
class Person
{
    public int Id { get; set; }
    public string name;
    public string Name { get; set; }
    public string Department { get; set; }
}
#endregion

#region Field And Property Access
//EF Core sorgulama sürecinde, entity içerisindeki property'lerin ya da field'ların kullanıp kullanılmayacağının davranışını bizlere belirtmektedir.
//Yani yukarıdaki gibi field'ın kullanılacağını belirtmiş olsakta, yine de property'i kullan diyebiliriz.

//EF Core, hiçbir ayarlama yoksa varsayılan olarak property'ler üzerinden verileri işler, eğer ki backing field bildiriliyorsa field üzerinden işler,
//yok eğer backing field bildirildiği halde davranış belirtiliyorsa en son davranış ne belirtilmişse ona göre işlemeyi devam ettirir.

//Burada kullanacağımız metodumuz: UsePropertyAccessMode üzerinden davranış modellemesi gerçekleştirilebilir.
#endregion

#region Field-Only Properties
//Çokomelli!!!

//Entity'lerde değerleri almak için property'ler yerine metotların kullanıldığı veya belirli alanların hiç gösterilmemesi gerektiği durumlarda
//(örneğin primary key kolonu) kullanabilir. Yani Person içerisinde Id property'si olsun ama property olarak erişilmesin, diyebiliriz.

class Person
{
    public int Id { get; set; }
    public string name;
    //Name property'sini tanımlamadık. Herhangi bir backing field çalışması yapamayız. Burada Fluent API kısmına gelip OnModelCreating'de işlemler yapmamız lazım.
    public string Department { get; set; }

    //OnModel'da gerekli işlemi yaptıktan sonra name field'ına istediğimiz gibi metotlarla veri ekleme ve okuma operasyonları yapabiliriz.
    public string GetName()
        => name;
    //Entity'lerin içerisindeki alanları metotlarla yönetmek istediğimizde Field-Only Properties'ı bu şekilde kullanabiliriz.
    public string SetName(string value)
        => this.name = value;
}
#endregion

class BackingFieldDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=BaskingFieldDb;User ID=SA;Password=1");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Fluent API yöntemi ile Backing Fields
        /*
        modelBuilder.Entity<Person>()
            .Property(p => p.Name) ////Person içerisindeki Name Property'sine gel
            .HasField(nameof(Person.name)) //Bu property'nin 1 tane backing field'ı var olsun adı da Person.name yani Person içerisindeki name field'ıdır.
            .UsePropertyAccessMode(PropertyAccessMode.PreferProperty);
        //Property'e erişim davranışını buradan PropertyAccessMode enum'u üzerinden belirleyebiliriz.

        //Field: Veri erişim süreçlerinde sadece field'ların kullanılmasını söyler. Eğer field'ın kullanılamayacağı durum söz konusu olursa
        //bir exception fırlatır.

        //FieldDuringConstruction: Veri erişim süreçlerinde ilgili entityden bir nesne oluşturulma sürecinde field'ların kullanılmasını söyler.

        //Property: Veri erişim sürecinde sadece propertynin kullanılmasını söyler. Eğer property'nin kullanılamayacağı durum söz konusuysa
        //(read-only, write-only) bir exception fırlatır.

        //Aynı şekilde,
        //PreferField,
        //PreferFieldDuringConstruction,
        //PreferProperty
        */

        //Field-Only Properties ile Backing Fields
        //name field'ına Person'daki bir property gibi davranmasını söylüyoruz.
        modelBuilder.Entity<Person>()
            .Property(nameof(Person.name)); 
        //Artık name field'ımıza Name'e karşılık gelen bütünde ğerler aktarılmış olacaktır.
    }
}
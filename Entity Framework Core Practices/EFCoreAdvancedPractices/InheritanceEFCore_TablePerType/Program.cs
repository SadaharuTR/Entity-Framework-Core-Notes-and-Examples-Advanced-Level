using Microsoft.EntityFrameworkCore;

ApplicationDbContext context = new();

#region Table Per Type (TPT) Nedir?
//Entity'lerin aralarında kalıtımsal ilişkiye sahip olduğu durumlarda her bir türe/entitye/tip/referans karşılık bir tablo generate eden davranıştır.
//Generate edilen bu tablolar hiyerarşik düzlemde kendi aralarında birebir ilişkiye sahiptir.
#endregion

#region TPT Nasıl Uygulanır?
//1-TPT'yi uygulayabilmek için öncelikle entity'lerin kendi aralarında olması gereken mantıkta inşa edilmesi gerekmektedir.
//2-Entity'ler DbSet olarak bildirilmelidir.
//3-Hiyerarşik olarak aralarında kalıtımsal ilişki olan tüm entityler OnModelCreating fonksiyonunda ToTable metodu ile konfigüre edilmelidir.
//Böylece EF Core kalıtımsal ilişki olan bu tablolar arasında TPT davranışının olduğunu anlayacaktır.
#endregion

#region TPT'de Veri Ekleme
/*
Technician technician = new() { Name = "Nazgül", Surname = "Efecore", Department = "Yazılım Departmanı", Branch = "C# Kodlama" };
await context.Technicians.AddAsync(technician);

Customer customer = new() { Name = "Kastım", Surname = "Ir", CompanyName = "Epic Store" };
await context.Customers.AddAsync(customer);
await context.SaveChangesAsync();
*/
#endregion

#region TPT'de Veri Silme

//Bir veriyi siliyorsak ounla ilişkili veriler de silinecektir. Cascade yapılanması devreye girecektir.
/*
Employee? silinecek = await context.Employees.FindAsync(5); //Employee'larda 3 Id'sine sahip data'yı elde et. Employee eklemememize rağmen yukarıdaki TEchnician-Customer
//eklemeleri sayesinde TPT davranışı kullanılmıştır ve verilerimiz bir bütün olduğu için bu veriler sadece bu tablolara dağıtılmıştır. Eğer ki ilgili tabloda unique
//olarak bir Id'si varsa o değere ait verileri bir bütün olarak getirecektir. Employee'un kalıtımsal ilişkisinin devamı olan alt bir ürünü eklediğimiz dolayısıyla Employee
//ile ilgili bir kayıt koyacağımızdan dolayı, o kaydı sorgularken de doğal olarak diğer verileri de elde edeceğiz.
context.Employees.Remove(silinecek);
await context.SaveChangesAsync();
*/ 

//Abstract Class olan bir Entity'e karşılık tablo oluşturmak TPT ile mümkün oldu. Bu tablo üzerinden istediğimiz sorguyu yaratıp alt sınıflar üzerinde değişiklik
//yapma şansımız vardır.

//Person silelim.
/*
Person? silinecekPerson = await context.Persons.FindAsync(1);
context.Persons.Remove(silinecekPerson);
await context.SaveChangesAsync();
*/
#endregion

#region TPT'de Veri Güncelleme

//Elde etmek istediğimiz veriyi ister Technician'dan ister Employee'dan git, ister Person'dan. 2 Id'sine sahip veriyi elde edebiliriz.
//Biz Technician'dan gidelim.
//Technician technician = await context.Technicians.FindAsync(6);
//technician.Id = 1;
//await context.SaveChangesAsync();

#endregion

#region TPT'de Veri Sorgulama

//Employee employee = new() { Name = "Fatih", Surname = "Sultan Mehmet", Department = "Osmanlı" };
//await context.Employees.AddAsync(employee);
//await context.SaveChangesAsync();

var technicians = await context.Technicians.ToListAsync();
var employees = await context.Employees.ToListAsync();

Console.WriteLine();

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
    public string? CompanyName { get; set; }
}
class Technician : Employee
{
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
        //buradan hiyerarşiyi manuel olarak ayarlamamız lazım.
        //Bu davranışla kalıtımsal hiyerarşiye tuttuğumuz bütün entityleri OnModelCreating -> ToTable ile konf. etmemiz buradaki modellemenin TPT olduğunu gösterir.
        modelBuilder.Entity<Person>().ToTable("Persons");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Customer>().ToTable("Customers");
        modelBuilder.Entity<Technician>().ToTable("Technicians");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=PC\\SQLEXPRESS;Database=ApplicationDb;User ID=SA;Password=1;TrustServerCertificate=True");
    }
}
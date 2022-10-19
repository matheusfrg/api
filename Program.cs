using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDBContext>();

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapGet("/", () => "Hello World!");
app.MapGet("/User", () => new { Name = "Matheus Garcia", Age = 40 });
app.MapGet("/AddHeader", (HttpResponse response) =>
{
    response.Headers.Add("Teste", "Matheus Garcia");
    return new { Name = "Matheus Garcia", Age = 40 };
});

//api.app.com/users?dateStart={date}?dateEnd={date}
app.MapGet("/Product", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>
{
    return dateStart + " - " + dateEnd;
});

app.MapGet("/ProductByHeader", (HttpRequest request) =>
{
    return request.Headers["product-id"].ToString();
});

app.MapGet("/Product/{id}", ([FromRoute] int id) =>
{
    var product = ProductRepository.GetBy(id);
    if (product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

app.MapPost("/Product", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created($"/Products/{product.Id}", product.Id);
});

app.MapPut("/Product", (Product product) =>
{
    var productSave = ProductRepository.GetBy(product.Id);
    productSave.Name = product.Name;
    return Results.Ok();
});

app.MapDelete("/Product/{id}", ([FromRoute] int id) =>
{
    var productSave = ProductRepository.GetBy(id);
    ProductRepository.Remove(productSave);
    return Results.Ok();
});

if (app.Environment.IsStaging())
    app.MapGet("/configuration/database", (IConfiguration configuration) =>
    {
        return Results.Ok($"{configuration["database:connection"]}:{configuration["database:port"]}");
    });

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product)
    {
        Products.Add(product);
    }

    public static Product GetBy(int id)
    {
        return Products.FirstOrDefault(p => p.Id == id);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Category {
    public int Id { get; set; }

    public string Name { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category {get; set;}
}

public class ApplicationDBContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder){
        builder.Entity<Product>()
        .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        builder.Entity<Product>()
        .Property(p => p.Name).HasMaxLength(120).IsRequired();
        builder.Entity<Product>()
        .Property(p => p.Code).HasMaxLength(20).IsRequired();
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
     => options.UseSqlServer(
        "Server=localhost;Database=Products;User Id=sa;Password=@Sql2019;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");

}
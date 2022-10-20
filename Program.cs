using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

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

app.MapGet("/Product/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id == id).First();
    if (product != null)
        return Results.Ok(product);
    return Results.NotFound();
});

app.MapPost("/Product", (ProductRequest request, ApplicationDbContext context) =>
{
    var category = context.Categories.Where(c => c.Id == request.CategoryId).First();
    var product = new Product
    {
        Code = request.Code,
        Name = request.Name,
        Description = request.Description,
        Category = category
    };
    if (request.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in request.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }
    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created($"/Products/{product.Id}", product.Id);
});

app.MapPut("/Product/{id}", ([FromRoute] int id, ProductRequest request, ApplicationDbContext context) =>
{
    var product = context.Products
        .Include(p => p.Category)
        .Include(p => p.Tags)
        .Where(p => p.Id == id).First();
    var category = context.Categories.Where(c => c.Id == request.CategoryId).First();

    product.Code = request.Code;
    product.Name = request.Name;
    product.Description = request.Description;

    product.Category = category;
    
    if (request.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in request.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }
    context.SaveChanges();
    return Results.Ok();
});

app.MapDelete("/Product/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
{
    var product = context.Products.Where(p => p.Id == id).First();

    context.Products.Remove(product);
    context.SaveChanges();
    return Results.Ok();
});

app.Run();
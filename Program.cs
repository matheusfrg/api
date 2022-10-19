using Microsoft.AspNetCore.Mvc;

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

app.MapGet("/Product/{id}", ([FromRoute] int id) =>
{
    var product = ProductRepository.GetBy(id);
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

app.Run();
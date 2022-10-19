using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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

app.MapGet("/Product/{id}", ([FromRoute] string id) =>
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

app.MapDelete("/Product/{id}", ([FromRoute] string id) =>
{
    var productSave = ProductRepository.GetBy(id);
    ProductRepository.Remove(productSave);
    return Results.Ok();
});

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; }

    public static void Add(Product product)
    {
        if (Products == null)
            Products = new List<Product>();

        Products.Add(product);
    }

    public static Product GetBy(string id)
    {
        return Products.FirstOrDefault(p => p.Id == id);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Product
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}
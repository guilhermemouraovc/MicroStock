using Inventory.Api.Data;
using Inventory.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// CRUD mínimo de produtos
app.MapPost("/products", async (Product p, AppDbContext db) =>
{
    db.Products.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{p.Id}", p);
});

app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync());

app.MapGet("/products/{id:guid}", async (Guid id, AppDbContext db) =>
    await db.Products.FindAsync(id) is { } p ? Results.Ok(p) : Results.NotFound());

app.MapPut("/products/{id:guid}", async (Guid id, Product input, AppDbContext db) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    p.Name = input.Name;
    p.Description = input.Description;
    p.Price = input.Price;
    p.StockQuantity = input.StockQuantity;
    await db.SaveChangesAsync();
    return Results.Ok(p);
});

app.MapDelete("/products/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var p = await db.Products.FindAsync(id);
    if (p is null) return Results.NotFound();
    db.Remove(p);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

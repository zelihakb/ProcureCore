using ErpCloud.Api.DTOs;
using ErpCloud.Api.Data;
using ErpCloud.Api.Models;
using Microsoft.EntityFrameworkCore;
using ErpCloud.Api.Validation;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ErpDatabase")
    ?? throw new InvalidOperationException("Database connection string is missing.");

builder.Services.AddDbContext<ErpDbContext>(options =>
    options.UseSqlServer(connectionString));
var app = builder.Build();

app.MapGet("/health", () => "ERP API is running");
app.MapGet("/api/purchase-requests", async (ErpDbContext db) =>
   await db.PurchaseRequests.AsNoTracking()
    .OrderBy(r => r.Id)
    .ToListAsync());
app.MapPost("/api/purchase-requests", async (CreatePurchaseRequestDto request, ErpDbContext db) =>
{

    var validationError = PurchaseRequestValidator.Validate(
    request.ProductName,
    request.Quantity
);

    if (validationError is not null)
        return Results.BadRequest(new { error = validationError });

    var entity = new PurchaseRequest(
    request.ProductName,
    request.Quantity
);

    db.PurchaseRequests.Add(entity);
    await db.SaveChangesAsync();

    return Results.Created($"/api/purchase-requests/{entity.Id}", entity);
});
app.MapGet("/api/purchase-requests/count", async (ErpDbContext db) =>
    await db.PurchaseRequests.CountAsync());
app.MapGet("/api/purchase-requests/{id:int}", async (int id, ErpDbContext db) =>
{
    var request = await db.PurchaseRequests
    .AsNoTracking()
    .FirstOrDefaultAsync(r => r.Id == id);

    if (request is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(request);
});
app.Run();
public partial class Program { }
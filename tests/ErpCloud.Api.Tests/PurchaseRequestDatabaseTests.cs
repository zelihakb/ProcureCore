using ErpCloud.Api.Data;
using ErpCloud.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ErpCloud.Api.Tests;

public class PurchaseRequestDatabaseTests
{
    [Fact]
    public async Task SavedRequest_CanBeReadFromSqlServer()
    {
        await using var sql = new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-latest"
        ).Build();

        await sql.StartAsync();

        var connection = new SqlConnectionStringBuilder(
            sql.GetConnectionString()
        )
        {
            InitialCatalog = "ErpCloudTests"
        };

        var options = new DbContextOptionsBuilder<ErpDbContext>()
            .UseSqlServer(connection.ConnectionString)
            .Options;
        int savedId;
        await using (var db = new ErpDbContext(options))
        {
            await db.Database.MigrateAsync();

            var request = new PurchaseRequest("Keyboard", 5);

            db.PurchaseRequests.Add(request);
            await db.SaveChangesAsync();

            Assert.True(request.Id > 0);

            savedId = request.Id;

            await db.SaveChangesAsync();
        }

        await using var secondDb = new ErpDbContext(options);

        var saved = await secondDb.PurchaseRequests
            .AsNoTracking()
            .SingleAsync(r => r.Id == savedId);

        Assert.Equal("Keyboard", saved.ProductName);
        Assert.Equal(5, saved.Quantity);
    }
}
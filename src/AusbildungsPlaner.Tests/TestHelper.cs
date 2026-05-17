using AusbildungsPlaner.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AusbildungsPlaner.Tests;

public static class TestHelper
{
    public static AppDbContext CreateTestDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var db = new AppDbContext(options);
        db.Database.OpenConnection();
        db.Database.EnsureCreated();
        return db;
    }
}

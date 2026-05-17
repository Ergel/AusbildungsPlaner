using AusbildungsPlaner.Web.Data;
using AusbildungsPlaner.Web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AusbildungsPlaner.Web.Services;

public class ThemaService(IDbContextFactory<AppDbContext> dbFactory)
{
    public async Task<List<Thema>> GetAlleThemenAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Themen.OrderBy(t => t.Kategorie).ThenBy(t => t.Titel).ToListAsync();
    }

    public async Task<Thema?> GetThemaAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Themen.FindAsync(id);
    }

    public async Task<Thema> ErstellenAsync(Thema thema)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Themen.Add(thema);
        await db.SaveChangesAsync();
        return thema;
    }

    public async Task AktualisierenAsync(Thema thema)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        db.Themen.Update(thema);
        await db.SaveChangesAsync();
    }

    public async Task LoeschenAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var thema = await db.Themen.FindAsync(id);
        if (thema is not null)
        {
            db.Themen.Remove(thema);
            await db.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetKategorienAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Themen.Select(t => t.Kategorie).Distinct().OrderBy(k => k).ToListAsync();
    }
}

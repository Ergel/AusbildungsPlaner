using AusbildungsPlaner.Web.Data;
using AusbildungsPlaner.Web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AusbildungsPlaner.Web.Services;

public class ThemaService(AppDbContext db)
{
    public async Task<List<Thema>> GetAlleThemenAsync()
        => await db.Themen.OrderBy(t => t.Kategorie).ThenBy(t => t.Titel).ToListAsync();

    public async Task<Thema?> GetThemaAsync(int id)
        => await db.Themen.FindAsync(id);

    public async Task<Thema> ErstellenAsync(Thema thema)
    {
        db.Themen.Add(thema);
        await db.SaveChangesAsync();
        return thema;
    }

    public async Task AktualisierenAsync(Thema thema)
    {
        db.Themen.Update(thema);
        await db.SaveChangesAsync();
    }

    public async Task LoeschenAsync(int id)
    {
        var thema = await db.Themen.FindAsync(id);
        if (thema is not null)
        {
            db.Themen.Remove(thema);
            await db.SaveChangesAsync();
        }
    }
}

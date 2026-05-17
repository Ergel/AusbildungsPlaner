using AusbildungsPlaner.Web.Data;
using AusbildungsPlaner.Web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AusbildungsPlaner.Web.Services;

public class SessionService(AppDbContext db)
{
    public async Task<List<Session>> GetSessionsAsync(int semesterId)
        => await db.Sessions
                   .Include(s => s.Thema)
                   .Include(s => s.Gastdozent)
                   .Where(s => s.SemesterId == semesterId)
                   .OrderBy(s => s.Datum).ThenBy(s => s.Uhrzeit)
                   .ToListAsync();

    public async Task<Session> ErstellenAsync(Session session)
    {
        db.Sessions.Add(session);
        await db.SaveChangesAsync();
        return session;
    }

    public async Task AktualisierenAsync(Session session)
    {
        db.Sessions.Update(session);
        await db.SaveChangesAsync();
    }

    public async Task LoeschenAsync(int id)
    {
        var session = await db.Sessions.FindAsync(id);
        if (session is not null)
        {
            db.Sessions.Remove(session);
            await db.SaveChangesAsync();
        }
    }
}

using AusbildungsPlaner.Web.Data;
using AusbildungsPlaner.Web.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AusbildungsPlaner.Web.Services;

public class GastdozentService(AppDbContext db, UserManager<AppUser> userManager)
{
    public async Task<List<AppUser>> GetAlleGastdozentenAsync()
    {
        var gastdozenten = await userManager.GetUsersInRoleAsync("Gastdozent");
        return [.. gastdozenten.OrderBy(u => u.Nachname).ThenBy(u => u.Vorname)];
    }

    public async Task<List<Session>> GetMeineSessionsAsync(string userId)
        => await db.Sessions
                   .Include(s => s.Thema)
                   .Include(s => s.Semester)
                   .Where(s => s.GastdozentId == userId)
                   .OrderBy(s => s.Datum)
                   .ToListAsync();
}

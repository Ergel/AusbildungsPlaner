using AusbildungsPlaner.Web.Data;
using AusbildungsPlaner.Web.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AusbildungsPlaner.Web.Services;

public class SemesterService(AppDbContext db)
{
    public async Task<List<Semester>> GetAlleSemesterAsync()
        => await db.Semester.OrderByDescending(s => s.StartDatum).ToListAsync();

    public async Task<Semester?> GetSemesterAsync(int id)
        => await db.Semester.Include(s => s.Sessions).ThenInclude(s => s.Thema)
                            .Include(s => s.Sessions).ThenInclude(s => s.Gastdozent)
                            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Semester> ErstellenAsync(Semester semester)
    {
        db.Semester.Add(semester);
        await db.SaveChangesAsync();
        return semester;
    }

    public async Task AktualisierenAsync(Semester semester)
    {
        db.Semester.Update(semester);
        await db.SaveChangesAsync();
    }

    public async Task LoeschenAsync(int id)
    {
        var semester = await db.Semester.FindAsync(id);
        if (semester is not null)
        {
            db.Semester.Remove(semester);
            await db.SaveChangesAsync();
        }
    }
}

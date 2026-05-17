using AusbildungsPlaner.Web.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AusbildungsPlaner.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Semester> Semester => Set<Semester>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Thema> Themen => Set<Thema>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Session>()
            .HasOne(s => s.Gastdozent)
            .WithMany()
            .HasForeignKey(s => s.GastdozentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

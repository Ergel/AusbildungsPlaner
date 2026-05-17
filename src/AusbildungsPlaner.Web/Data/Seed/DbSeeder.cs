using Microsoft.AspNetCore.Identity;

namespace AusbildungsPlaner.Web.Data.Seed;

public static class DbSeeder
{
    public static readonly string[] Rollen = ["Admin", "Azubi", "Gastdozent"];

    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var rolle in Rollen)
        {
            if (!await roleManager.RoleExistsAsync(rolle))
                await roleManager.CreateAsync(new IdentityRole(rolle));
        }
    }
}

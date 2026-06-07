using System.Net;
using AusbildungsPlaner.Web.Components;
using AusbildungsPlaner.Web.Data;
using AusbildungsPlaner.Web.Data.Models;
using AusbildungsPlaner.Web.Data.Seed;
using AusbildungsPlaner.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using MudBlazor.Services;

namespace AusbildungsPlaner.UITests;

/// <summary>
/// Startet die Blazor-App auf einem echten Kestrel-Port und stellt
/// einen Playwright-Browser für UI-Tests bereit.
/// </summary>
public class PlaywrightFixture : IAsyncLifetime
{
    private readonly string _dbPfad = Path.Combine(Path.GetTempPath(), $"uitests_{Guid.NewGuid():N}.db");
    private WebApplication? _app;

    public const string AdminEmail = "admin@test.de";
    public const string AdminPasswort = "Admin1234!";

    private IPlaywright? _playwright;
    public IBrowser Browser { get; private set; } = null!;
    public string BasisUrl { get; private set; } = string.Empty;

    // -----------------------------------------------------------------
    // App hochfahren
    // -----------------------------------------------------------------
    public async Task InitializeAsync()
    {
        _app = AppFuerTestsBauen(_dbPfad);
        await _app.StartAsync();
        BasisUrl = _app.Urls.First();

        using var scope = _app.Services.CreateScope();
        await TestdatenAnlegenAsync(scope.ServiceProvider);

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
        });
    }

    /// <summary>
    /// Baut eine WebApplication mit Test-Datenbank und zufälligem Port.
    /// Spiegelt die Konfiguration aus Program.cs, ersetzt aber die DB.
    /// </summary>
    private static WebApplication AppFuerTestsBauen(string dbPfad)
    {
        var builder = WebApplication.CreateBuilder();

        // Zufälligen freien Port wählen
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        builder.WebHost.UseUrls($"http://localhost:{port}");

        // Services – identisch zu Program.cs
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddMudServices();

        // Test-Datenbank statt Produktions-DB
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPfad}")
                   .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

        builder.Services.AddScoped<ThemaService>();
        builder.Services.AddScoped<SemesterService>();
        builder.Services.AddScoped<SessionService>();
        builder.Services.AddScoped<GastdozentService>();

        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddCascadingAuthenticationState();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseStaticFiles(); // MapStaticAssets() braucht Web-Build-Manifest – für Tests reicht UseStaticFiles
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        // Test-Hilfsendpunkt: Login ohne Formular (nur im Test-Server verfügbar)
        app.MapGet("/test/login/{email}", async (
            string email,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) return Results.NotFound($"Nutzer '{email}' nicht gefunden");
            await signInManager.SignInAsync(user, isPersistent: false);
            return Results.Ok("Angemeldet");
        }).AllowAnonymous();

        return app;
    }

    private static async Task TestdatenAnlegenAsync(IServiceProvider services)
    {
        // Datenbank anlegen
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        // Rollen anlegen
        await DbSeeder.SeedRolesAsync(services);

        // Admin-Testnutzer anlegen
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        if (await userManager.FindByEmailAsync(AdminEmail) is null)
        {
            var admin = new AppUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                Vorname = "Test",
                Nachname = "Admin",
                EmailConfirmed = true,
            };
            var result = await userManager.CreateAsync(admin, AdminPasswort);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Testnutzer-Erstellung fehlgeschlagen: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }

    // -----------------------------------------------------------------
    // Herunterfahren
    // -----------------------------------------------------------------
    public async Task DisposeAsync()
    {
        await Browser.DisposeAsync();
        _playwright?.Dispose();

        if (_app != null)
            await _app.DisposeAsync();

        if (File.Exists(_dbPfad))
            File.Delete(_dbPfad);
    }

    // -----------------------------------------------------------------
    // Hilfsmethoden
    // -----------------------------------------------------------------

    /// <summary>Neue Browser-Seite mit gesetzter BaseURL.</summary>
    public async Task<IPage> NeueSeiteMitBaseUrlAsync()
    {
        var kontext = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = BasisUrl,
        });
        return await kontext.NewPageAsync();
    }

    /// <summary>Öffnet eine Seite und meldet den Admin-Testnutzer an.</summary>
    /// <summary>
    /// Meldet den Admin per Test-Endpoint an (zuverlässig, ohne Formular-Submit).
    /// Nur für Tests die eine angemeldete Seite als Vorbedingung brauchen.
    /// </summary>
    public async Task<IPage> AngemeldeteSeiteFuerAdminAsync()
    {
        var seite = await NeueSeiteMitBaseUrlAsync();
        // Test-Hilfsendpunkt setzt das Auth-Cookie direkt im Browser-Kontext
        await seite.GotoAsync($"/test/login/{Uri.EscapeDataString(AdminEmail)}");
        await seite.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.DOMContentLoaded);
        return seite;
    }
}

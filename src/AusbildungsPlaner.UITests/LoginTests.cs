using Microsoft.Playwright;

namespace AusbildungsPlaner.UITests;

public class LoginTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public LoginTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Gegeben_AnmeldeSeite_Wenn_GueltigeZugangsdatenEingegeben_Dann_LoginFormularVerschwindet()
    {
        // Arrange
        var seite = await _fixture.NeueSeiteMitBaseUrlAsync();
        await seite.GotoAsync("/account/login");
        await seite.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await seite.Locator("button[type='submit']").WaitForAsync();

        // Act
        await seite.Locator("input[type='email']").FillAsync(PlaywrightFixture.AdminEmail);
        await seite.Locator("input[type='password']").FillAsync(PlaywrightFixture.AdminPasswort);
        await seite.Locator("button[type='submit']").ClickAsync();
        await seite.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert: kein Login-Formular mehr sichtbar
        var istNochAufLogin = await seite.Locator("input[type='password']").IsVisibleAsync();
        Assert.False(istNochAufLogin, $"Nach Login noch auf: {seite.Url}");
    }

    [Fact]
    public async Task Gegeben_AnmeldeSeite_Wenn_FalschesPasswortEingegeben_Dann_FehlermeldungAngezeigt()
    {
        // Arrange
        var seite = await _fixture.NeueSeiteMitBaseUrlAsync();
        await seite.GotoAsync("/account/login");
        await seite.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Act
        await seite.Locator("input[type='email']").FillAsync(PlaywrightFixture.AdminEmail);
        await seite.Locator("input[type='password']").FillAsync("FalschesPasswort!");
        await seite.Locator("button[type='submit']").ClickAsync();
        await seite.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Assert: Fehlermeldung im Seiteninhalt (MudAlert wird per SSR gerendert)
        var seitentext = await seite.InnerTextAsync("body");
        Assert.Contains("Ungültige", seitentext);
    }

    [Fact]
    public async Task Gegeben_NichtAngemeldeterNutzer_Wenn_GeschuetzteSeiteAufgerufen_Dann_HinweismeldungAngezeigt()
    {
        // Arrange
        var seite = await _fixture.NeueSeiteMitBaseUrlAsync();

        // Act
        await seite.GotoAsync("/themen");
        await seite.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Assert: AuthorizeRouteView rendert Hinweis in den Body
        var seitentext = await seite.InnerTextAsync("body");
        Assert.Contains("nicht angemeldet", seitentext);
    }
}

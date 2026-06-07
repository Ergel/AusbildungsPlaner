namespace AusbildungsPlaner.UITests;

public class ThemenTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public ThemenTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Gegeben_AdminIstAngemeldet_Wenn_ThemenSeiteAufgerufen_Dann_TitelUndSuchfeldSichtbar()
    {
        // Arrange
        var seite = await _fixture.AngemeldeteSeiteFuerAdminAsync();

        // Act
        await seite.GotoAsync("/themen");
        await seite.WaitForSelectorAsync("h4, .mud-text");

        // Assert: Seitenüberschrift vorhanden
        var seitentext = await seite.TextContentAsync("body");
        Assert.Contains("Themen", seitentext);

        // Assert: Suchfeld vorhanden
        var suchfeld = seite.Locator("input[placeholder='Suchen...']");
        await suchfeld.WaitForAsync();
        Assert.True(await suchfeld.IsVisibleAsync());
    }

    [Fact]
    public async Task Gegeben_AdminIstAngemeldet_Wenn_ThemenSeiteAufgerufen_Dann_ButtonNeuesThemaSichtbar()
    {
        // Arrange
        var seite = await _fixture.AngemeldeteSeiteFuerAdminAsync();

        // Act
        await seite.GotoAsync("/themen");

        // Assert: "Neues Thema"-Button nur für Admin sichtbar
        var button = seite.Locator("button:has-text('Neues Thema')");
        await button.WaitForAsync();
        Assert.True(await button.IsVisibleAsync());
    }

    [Fact]
    public async Task Gegeben_AdminIstAngemeldet_Wenn_NeuesThemaGespeichert_Dann_ThemaInTabelleAngezeigt()
    {
        // Arrange
        var seite = await _fixture.AngemeldeteSeiteFuerAdminAsync();
        await seite.GotoAsync("/themen");

        // Act: Dialog öffnen
        await seite.Locator("button:has-text('Neues Thema')").ClickAsync();
        await seite.WaitForSelectorAsync(".mud-dialog");

        // Felder ausfüllen
        var dialog = seite.Locator(".mud-dialog");
        await dialog.Locator("input").Nth(0).FillAsync("Playwright Test-Thema");
        await dialog.Locator("input").Nth(1).FillAsync("Testing");

        // Speichern
        await dialog.Locator("button:has-text('Speichern')").ClickAsync();
        await seite.WaitForSelectorAsync(".mud-dialog", new() { State = Microsoft.Playwright.WaitForSelectorState.Hidden });

        // Assert: neues Thema in Tabelle sichtbar
        var tabellentext = await seite.TextContentAsync(".mud-table, .mud-data-grid");
        Assert.Contains("Playwright Test-Thema", tabellentext);
    }
}

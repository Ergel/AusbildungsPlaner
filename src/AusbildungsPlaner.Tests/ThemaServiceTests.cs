using AusbildungsPlaner.Web.Data.Models;
using AusbildungsPlaner.Web.Services;

namespace AusbildungsPlaner.Tests;

public class ThemaServiceTests
{
    [Fact]
    public async Task GetThemenNachKategorieAsync_MitPassenderKategorie_GibtNurPassendeThemenZurueck()
    {
        // Arrange
        using var db = TestHelper.CreateTestDb();
        db.Themen.AddRange(
            new Thema { Titel = "Vererbung", Kategorie = "OOP" },
            new Thema { Titel = "Interfaces", Kategorie = "OOP" },
            new Thema { Titel = "SELECT JOIN", Kategorie = "Datenbanken" }
        );
        await db.SaveChangesAsync();

        var service = new ThemaService(db);

        // Act
        var ergebnis = await service.GetThemenNachKategorieAsync("OOP");

        // Assert
        Assert.Equal(2, ergebnis.Count);
        Assert.All(ergebnis, t => Assert.Equal("OOP", t.Kategorie));
    }

    [Fact]
    public async Task GetThemenNachKategorieAsync_UnbekannteKategorie_GibtLeereListeZurueck()
    {
        // Arrange
        using var db = TestHelper.CreateTestDb();
        db.Themen.Add(new Thema { Titel = "Vererbung", Kategorie = "OOP" });
        await db.SaveChangesAsync();

        var service = new ThemaService(db);

        // Act
        var ergebnis = await service.GetThemenNachKategorieAsync("Testing");

        // Assert
        Assert.Empty(ergebnis);
    }

    [Fact]
    public async Task GetThemenNachKategorieAsync_ErgebnisSortiert_NachTitel()
    {
        // Arrange
        using var db = TestHelper.CreateTestDb();
        db.Themen.AddRange(
            new Thema { Titel = "Polymorphismus", Kategorie = "OOP" },
            new Thema { Titel = "Abstrakte Klassen", Kategorie = "OOP" }
        );
        await db.SaveChangesAsync();

        var service = new ThemaService(db);

        // Act
        var ergebnis = await service.GetThemenNachKategorieAsync("OOP");

        // Assert
        Assert.Equal("Abstrakte Klassen", ergebnis[0].Titel);
        Assert.Equal("Polymorphismus", ergebnis[1].Titel);
    }
}

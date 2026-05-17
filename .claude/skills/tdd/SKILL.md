---
name: tdd
description: >
  Test-Driven Development (TDD) für das AusbildungsPlaner-Projekt.
  Verwende diesen Skill immer wenn der Nutzer einen neuen Service, eine neue Methode,
  ein neues Model oder ein neues Feature implementieren möchte — auch wenn der Nutzer
  nicht explizit "TDD" oder "Test" sagt. Der Skill stellt sicher, dass zuerst ein
  xUnit-Test geschrieben wird, bevor Produktionscode entsteht.
---

# TDD-Workflow für AusbildungsPlaner

## Reihenfolge: Test zuerst, immer

Red → Green → Refactor. Niemals Produktionscode schreiben bevor ein fehlschlagender Test existiert.

## Schritt 1: Test-Projekt sicherstellen

Prüfe ob `src/AusbildungsPlaner.Tests/` existiert. Falls nicht, lege es an:

```powershell
& "C:\Program Files\dotnet\dotnet.exe" new xunit -n AusbildungsPlaner.Tests -o src/AusbildungsPlaner.Tests
& "C:\Program Files\dotnet\dotnet.exe" sln add src/AusbildungsPlaner.Tests
& "C:\Program Files\dotnet\dotnet.exe" add src/AusbildungsPlaner.Tests reference src/AusbildungsPlaner.Web
& "C:\Program Files\dotnet\dotnet.exe" add src/AusbildungsPlaner.Tests package Microsoft.EntityFrameworkCore.Sqlite
```

Für Services die `AppDbContext` brauchen, wird eine In-Memory-SQLite-Datenbank verwendet:

```csharp
// TestHelper.cs
public static AppDbContext CreateTestDb()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite("Data Source=:memory:")
        .Options;
    var db = new AppDbContext(options);
    db.Database.OpenConnection();
    db.Database.EnsureCreated();
    return db;
}
```

## Schritt 2: Test schreiben (RED)

Schreibe den Test **bevor** die Implementierung existiert. Benenne Tests nach dem Schema:
`MethodenName_Szenario_ErwartetesErgebnis`

Beispiel für einen Service-Test:

```csharp
public class ThemaServiceTests
{
    [Fact]
    public async Task ErstellenAsync_NeuesThema_WirdInDatenbankGespeichert()
    {
        // Arrange
        using var db = TestHelper.CreateTestDb();
        var service = new ThemaService(db);
        var thema = new Thema { Titel = "OOP Grundlagen", Kategorie = "OOP" };

        // Act
        await service.ErstellenAsync(thema);

        // Assert
        var gespeichert = await db.Themen.FindAsync(thema.Id);
        Assert.NotNull(gespeichert);
        Assert.Equal("OOP Grundlagen", gespeichert.Titel);
    }
}
```

Führe den Test aus — er muss **rot** sein (Kompilierfehler oder Fehlschlag):

```powershell
& "C:\Program Files\dotnet\dotnet.exe" test src/AusbildungsPlaner.Tests --verbosity normal
```

## Schritt 3: Minimalen Produktionscode schreiben (GREEN)

Schreibe nur so viel Code wie nötig damit der Test grün wird. Keine zusätzlichen Features.

Führe Tests erneut aus — alle müssen grün sein.

## Schritt 4: Refactoring

Bereinige den Code ohne Tests zu brechen. Tests nach jedem Refactoring-Schritt ausführen.

## Was zu testen ist

| Komponente | Testziel |
|-----------|----------|
| Services | Geschäftslogik, CRUD-Operationen, Filterung |
| Models | Berechnete Properties (z.B. `VollerName`) |
| Validierung | Pflichtfelder, Grenzwerte |

**Nicht testen:** Razor Components (zu aufwändig für dieses Projekt), EF-Migrations, Identity-Infrastruktur.

## Checkliste vor jeder Implementierung

- [ ] Test geschrieben und er schlägt fehl (Red)
- [ ] Minimale Implementierung macht Test grün (Green)
- [ ] Code bereinigt, Tests noch grün (Refactor)
- [ ] `dotnet test` läuft ohne Fehler durch

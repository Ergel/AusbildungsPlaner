# AusbildungsPlaner

Eine Blazor Server-Webanwendung zur Verwaltung von Ausbildungssessions für Fachinformatiker Anwendungsentwicklung.

## Features

- **Semesterplanung** — Semester anlegen und Sessions verwalten (Datum, Uhrzeit, Dauer, Raum/Zoom)
- **Themenverwaltung** — Themen nach Kategorien (z.B. OOP, Datenbanken, Testing) erstellen und zuweisen
- **Gastdozenten** — Gastdozenten zu Sessions zuweisen; eigene Sessions-Ansicht für Gastdozenten
- **Rollen** — Admin (Ausbilder), Azubi (lesend), Gastdozent (eigene Sessions)
- **Kalenderansicht** — Sessions als Monatskalender oder Liste anzeigen

## Tech Stack

| Schicht | Technologie |
|---------|-------------|
| Frontend + Backend | Blazor Server (.NET 9) |
| UI-Komponenten | MudBlazor 9 |
| ORM | Entity Framework Core 9 |
| Datenbank | SQLite |
| Auth | ASP.NET Core Identity |

## Voraussetzungen

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Git

## Lokaler Start

```bash
git clone https://github.com/Ergel/AusbildungsPlaner.git
cd AusbildungsPlaner
dotnet run --project src/AusbildungsPlaner.Web
```

Die App ist dann unter `https://localhost:5001` erreichbar.

Beim ersten Start werden automatisch:
- Die Datenbank (`ausbildungsplaner.db`) erstellt und migriert
- Die Rollen Admin, Azubi und Gastdozent angelegt

Einen Admin-Benutzer kannst du über die Registrierungsseite anlegen und die Rolle anschließend direkt in der Datenbank zuweisen.

## Projektstruktur

```
AusbildungsPlaner/
├── src/
│   └── AusbildungsPlaner.Web/
│       ├── Components/         # Razor Components (Pages, Layout, Dialoge)
│       ├── Data/               # EF Core DbContext, Modelle, Migrations, Seed
│       └── Services/           # Business-Logic-Schicht
└── .github/
    └── workflows/ci.yml        # GitHub Actions: Build + Test
```

## CI

GitHub Actions führt bei jedem Push und Pull Request auf `main` automatisch `dotnet build` und `dotnet test` aus.

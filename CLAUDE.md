# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```powershell
# dotnet is at C:\Program Files\dotnet\dotnet.exe — use full path in PowerShell
& "C:\Program Files\dotnet\dotnet.exe" build src/AusbildungsPlaner.Web
& "C:\Program Files\dotnet\dotnet.exe" run --project src/AusbildungsPlaner.Web

# Kill running app before rebuilding (locks the .exe)
Stop-Process -Name "AusbildungsPlaner.Web" -ErrorAction SilentlyContinue

# EF migrations
dotnet ef migrations add <Name> --project src/AusbildungsPlaner.Web
dotnet ef database update --project src/AusbildungsPlaner.Web
```

The SQLite database (`ausbildungsplaner.db`) is created automatically on first run via `MigrateAsync()` in `Program.cs`. The file is gitignored.

## Architecture

Blazor Server (.NET 9) with interactive server-side rendering. No separate API — services are called directly from Razor components via DI.

**Data flow:** `Page.razor` → `XyzService` → `AppDbContext` (EF Core) → `ausbildungsplaner.db`

**Service layer** (`Services/`) — one service per aggregate, all registered as `Scoped`:
- `ThemaService` — CRUD for Thema
- `SemesterService` — CRUD for Semester, includes Sessions with Thema + Gastdozent
- `SessionService` — CRUD for Session (scoped to a SemesterId)
- `GastdozentService` — uses `UserManager<AppUser>` to get users in the "Gastdozent" role

**Dialogs** — edit forms live in `*Dialog.razor` files (e.g. `SessionDialog`, `ThemaDialog`). They receive entity parameters, deep-copy them in `OnInitialized` to prevent side effects on cancel, and return the modified entity via `MudDialog.Close(DialogResult.Ok(entity))`. The calling page handles the actual save.

**Auth:**
- Three roles seeded at startup: `Admin`, `Azubi`, `Gastdozent` (see `Data/Seed/DbSeeder.cs`)
- `AppUser` extends `IdentityUser` with `Vorname`, `Nachname`, `VollerName`
- Pages use `@attribute [Authorize]` or `@attribute [Authorize(Roles = "Admin")]`
- UI conditionals use `<AuthorizeView Roles="Admin">` — when nesting inside `MudDataGrid` `CellTemplate`, add `Context="authCtx"` to avoid the implicit `context` conflict

## Key Conventions

- **Language:** All UI strings, variable names, method names, and comments are in German
- **Nullable:** Project has `<Nullable>enable</Nullable>` — non-nullable properties use `= null!` for EF navigation properties
- **No `AddDbContextFactory`** — only `AddDbContext` is registered. Using `IDbContextFactory` alongside it causes `InvalidOperationException` at startup
- **MudBlazor forms:** Use `await _form.ValidateAsync()` (not the obsolete `Validate()`)
- **`AuthorizeView` context naming:** When `AuthorizeView` is nested inside a `TemplateColumn CellTemplate`, the outer `context` refers to the grid row — add `Context="authCtx"` on the `AuthorizeView` to disambiguate
- **`MudDialog` confirmation:** Use `ShowMessageBoxAsync` (MudBlazor 9 renamed from `ShowMessageBox`)

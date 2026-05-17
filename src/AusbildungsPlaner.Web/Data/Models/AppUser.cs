using Microsoft.AspNetCore.Identity;

namespace AusbildungsPlaner.Web.Data.Models;

public class AppUser : IdentityUser
{
    public string Vorname { get; set; } = string.Empty;
    public string Nachname { get; set; } = string.Empty;
    public string VollerName => $"{Vorname} {Nachname}";
}

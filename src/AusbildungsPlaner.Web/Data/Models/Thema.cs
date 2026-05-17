namespace AusbildungsPlaner.Web.Data.Models;

public class Thema
{
    public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Beschreibung { get; set; } = string.Empty;
    public string Kategorie { get; set; } = string.Empty;

    public ICollection<Session> Sessions { get; set; } = [];
}

namespace AusbildungsPlaner.Web.Data.Models;

public enum SessionTyp { Praesenz, Zoom }

public class Session
{
    public int Id { get; set; }
    public DateOnly Datum { get; set; }
    public TimeOnly Uhrzeit { get; set; }
    public int DauerMinuten { get; set; } = 120;
    public SessionTyp Typ { get; set; }
    public string? Raum { get; set; }
    public string? ZoomLink { get; set; }
    public string? Notizen { get; set; }

    public int SemesterId { get; set; }
    public Semester Semester { get; set; } = null!;

    public int? ThemaId { get; set; }
    public Thema? Thema { get; set; }

    public string? GastdozentId { get; set; }
    public AppUser? Gastdozent { get; set; }
}

namespace AusbildungsPlaner.Web.Data.Models;

public class Semester
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDatum { get; set; }
    public DateOnly EndDatum { get; set; }

    public ICollection<Session> Sessions { get; set; } = [];
}

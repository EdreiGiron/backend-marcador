public class Player
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Number { get; set; }
    public string Position { get; set; } = string.Empty;
    public double Height { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; } = string.Empty;

    public int TeamId { get; set; }
    public Team? Team { get; set; }
}
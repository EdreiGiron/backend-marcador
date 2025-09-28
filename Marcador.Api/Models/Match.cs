namespace Marcador.Api.Models;

public class Match
{
    public int Id { get; set; }
    public DateTime MatchDate { get; set; }

    // Relación con equipos
    public int HomeTeamId { get; set; }
    public Team? HomeTeam { get; set; }

    public int AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }

    // Resultado
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
}

namespace Marcador.Api.Domain;

public class GameSnapshot
{
    // Identificador del partido; lo usaremos también como "sala" en SignalR
    public string MatchId { get; set; } = "";

    public TeamState Home { get; set; } = new();
    public TeamState Away { get; set; } = new();

    public int Quarter { get; set; } = 1;                 // 1..4
    public int QuarterDurationMs { get; set; } = 10*60*1000;
    public int TimeLeftMs { get; set; }                    // tiempo restante al guardar

    // "home" | "away" | null
    public string? Possession { get; set; }

    public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
}

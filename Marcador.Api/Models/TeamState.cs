namespace Marcador.Api.Models;

public class TeamState
{
    public string Name { get; set; } = "EQUIPO";
    public int Score { get; set; }
    public int Fouls { get; set; }

    // Timeouts por reglamento (ajústalo si usas otros valores)
    public int Timeouts30 { get; set; } = 2;
    public int Timeouts60 { get; set; } = 2;
}

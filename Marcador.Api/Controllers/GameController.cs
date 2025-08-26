using Marcador.Api.Domain;
using Marcador.Api.Infra;
using Microsoft.AspNetCore.Mvc;

namespace Marcador.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : ControllerBase
{
    private readonly AppDbContext _db;
    public GameController(AppDbContext db) => _db = db;

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] GameSnapshot dto)
    {
        dto.SavedAtUtc = DateTime.UtcNow;
        _db.Games.Add(dto);
        await _db.SaveChangesAsync();
        return Ok(new { ok = true });
    }
}

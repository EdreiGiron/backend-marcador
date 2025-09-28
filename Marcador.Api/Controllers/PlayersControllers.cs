using Marcador.Api.Models;
using Marcador.Api.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marcador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly AppDbContext _db;
    public PlayersController(AppDbContext db) => _db = db;

    // 👀 Cualquier usuario autenticado puede ver jugadores
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetPlayers()
    {
        var players = await _db.Set<Player>()
                               .Include(p => p.Team)
                               .ToListAsync();
        return Ok(players);
    }

    // 👀 Cualquier usuario autenticado puede ver un jugador específico
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayer(int id)
    {
        var player = await _db.Set<Player>()
                              .Include(p => p.Team)
                              .FirstOrDefaultAsync(p => p.Id == id);
        if (player == null) return NotFound();
        return Ok(player);
    }

    // 🔐 Solo Admin puede crear jugadores
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromBody] Player player)
    {
        _db.Set<Player>().Add(player);
        await _db.SaveChangesAsync();
        return Ok(player);
    }

    // 🔐 Solo Admin puede editar jugadores
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(int id, [FromBody] Player updated)
    {
        var player = await _db.Set<Player>().FindAsync(id);
        if (player == null) return NotFound();

        player.FullName = updated.FullName;
        player.Number = updated.Number;
        player.Position = updated.Position;
        player.Height = updated.Height;
        player.Age = updated.Age;
        player.Nationality = updated.Nationality;
        player.TeamId = updated.TeamId;

        await _db.SaveChangesAsync();
        return Ok(player);
    }

    // 🔐 Solo Admin puede eliminar jugadores
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        var player = await _db.Set<Player>().FindAsync(id);
        if (player == null) return NotFound();

        _db.Set<Player>().Remove(player);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Jugador eliminado" });
    }
}

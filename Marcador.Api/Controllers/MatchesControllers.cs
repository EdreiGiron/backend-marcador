using Marcador.Api.Models;
using Marcador.Api.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marcador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly AppDbContext _db;
    public MatchesController(AppDbContext db) => _db = db;

    // 👀 Cualquier usuario autenticado puede ver los partidos
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMatches()
    {
        var matches = await _db.Set<Match>()
                               .Include(m => m.HomeTeam)
                               .Include(m => m.AwayTeam)
                               .ToListAsync();
        return Ok(matches);
    }

    // 👀 Cualquier usuario autenticado puede ver un partido específico
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMatch(int id)
    {
        var match = await _db.Set<Match>()
                             .Include(m => m.HomeTeam)
                             .Include(m => m.AwayTeam)
                             .FirstOrDefaultAsync(m => m.Id == id);
        if (match == null) return NotFound();
        return Ok(match);
    }

    // 🔐 Solo Admin puede crear partidos
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] Match match)
    {
        _db.Set<Match>().Add(match);
        await _db.SaveChangesAsync();
        return Ok(match);
    }

    // 🔐 Solo Admin puede editar partidos (ej. asignar resultado)
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMatch(int id, [FromBody] Match updated)
    {
        var match = await _db.Set<Match>().FindAsync(id);
        if (match == null) return NotFound();

        match.MatchDate = updated.MatchDate;
        match.HomeTeamId = updated.HomeTeamId;
        match.AwayTeamId = updated.AwayTeamId;
        match.HomeScore = updated.HomeScore;
        match.AwayScore = updated.AwayScore;

        await _db.SaveChangesAsync();
        return Ok(match);
    }

    // 🔐 Solo Admin puede eliminar partidos
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMatch(int id)
    {
        var match = await _db.Set<Match>().FindAsync(id);
        if (match == null) return NotFound();

        _db.Set<Match>().Remove(match);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Partido eliminado" });
    }
}

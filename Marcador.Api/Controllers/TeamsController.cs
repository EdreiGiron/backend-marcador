using Marcador.Api.Models;
using Marcador.Api.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marcador.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _db;
    public TeamsController(AppDbContext db) => _db = db;

    // Cualquier usuario autenticado puede ver los equipos
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetTeams()
    {
        var teams = await _db.Set<Team>().ToListAsync();
        return Ok(teams);
    }

    // Cualquier usuario autenticado puede ver un equipo en específico
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeam(int id)
    {
        var team = await _db.Set<Team>().FindAsync(id);
        if (team == null) return NotFound();
        return Ok(team);
    }

    // Solo Admin puede crear equipos
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] Team team)
    {
        _db.Set<Team>().Add(team);
        await _db.SaveChangesAsync();
        return Ok(team);
    }

    // Solo Admin puede editar equipos
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int id, [FromBody] Team updated)
    {
        var team = await _db.Set<Team>().FindAsync(id);
        if (team == null) return NotFound();

        team.Name = updated.Name;
        team.City = updated.City;
        team.LogoUrl = updated.LogoUrl;

        await _db.SaveChangesAsync();
        return Ok(team);
    }

    // Solo Admin puede eliminar equipos
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        var team = await _db.Set<Team>().FindAsync(id);
        if (team == null) return NotFound();

        _db.Set<Team>().Remove(team);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Equipo eliminado" });
    }
}

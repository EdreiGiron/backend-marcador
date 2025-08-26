using Marcador.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Marcador.Api.Infra;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<GameSnapshot> Games => Set<GameSnapshot>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Clave compuesta: mismo partido puede tener varios "guardados" en el tiempo
        mb.Entity<GameSnapshot>().HasKey(x => new { x.MatchId, x.SavedAtUtc });

        // TeamState como tipos "propietarios" del snapshot
        mb.Entity<GameSnapshot>().OwnsOne(x => x.Home);
        mb.Entity<GameSnapshot>().OwnsOne(x => x.Away);
    }
}

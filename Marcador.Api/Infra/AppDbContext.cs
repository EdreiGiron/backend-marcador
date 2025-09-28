using Marcador.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Marcador.Api.Infra;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    // Tablas principales
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Match> Matches => Set<Match>();

    // Marcador en vivo
    public DbSet<GameSnapshot> Games => Set<GameSnapshot>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // --- GameSnapshot ---
        mb.Entity<GameSnapshot>().HasKey(x => new { x.MatchId, x.SavedAtUtc });
        mb.Entity<GameSnapshot>().OwnsOne(x => x.Home);
        mb.Entity<GameSnapshot>().OwnsOne(x => x.Away);

        // --- User ---
        mb.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Role).HasMaxLength(50);
        });

        // --- Team ---
        mb.Entity<Team>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(150);
            entity.Property(t => t.City).HasMaxLength(150);
        });

        // --- Player ---
        mb.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.FullName).IsRequired().HasMaxLength(200);
            entity.HasOne(p => p.Team)
                  .WithMany()
                  .HasForeignKey(p => p.TeamId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Match ---
        mb.Entity<Match>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasOne(m => m.HomeTeam)
                  .WithMany()
                  .HasForeignKey(m => m.HomeTeamId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.AwayTeam)
                  .WithMany()
                  .HasForeignKey(m => m.AwayTeamId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

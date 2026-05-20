using Microsoft.EntityFrameworkCore;
using SoClover.Server.Models;
using System.Numerics;

namespace SoClover.Server.Context
{
    public class SoCloverDBContext : DbContext
    {
        public SoCloverDBContext(DbContextOptions<SoCloverDBContext> options) : base(options) { }

        public DbSet<GameRoom> GameRooms { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<BoardSlot> BoardSlots { get; set; }
        public DbSet<GameRoomCard> GameRoomCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameRoom>()
                .HasMany(g => g.Players)
                .WithOne(p => p.GameRoom)
                .HasForeignKey(p => p.GameRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameRoom>()
                .HasOne(g => g.CheckedPlayer)
                .WithMany()
                .HasForeignKey(g => g.CheckedPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameRoomCard>().HasOne(grc => grc.GameRoom)
                .WithMany(g => g.GameRoomCards)
                .HasForeignKey(grc => grc.GameRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Board)
                .WithOne(b => b.Player)
                .HasForeignKey<Board>(b => b.PlayerId) 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Board>()
                .HasMany(b => b.BoardSlots)
                .WithOne(s => s.Board)
                .HasForeignKey(s => s.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BoardSlot>()
                .HasOne(s => s.GameRoomCard)
                .WithOne(grc => grc.BoardSlot)
                .HasForeignKey<BoardSlot>(s => s.GameRoomCardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BoardSlot>()
                .HasOne(s => s.TargetGameRoomCard)
                .WithOne(grc => grc.TargetBoardSlot)
                .HasForeignKey<BoardSlot>(s => s.TargetGameRoomCardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameRoomCard>().HasOne(grc => grc.Card)
                .WithMany(c => c.GameRoomCards) 
                .HasForeignKey(grc => grc.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            //indexes
            modelBuilder.Entity<GameRoom>().HasIndex(g => g.RoomCode) .IsUnique();
            modelBuilder.Entity<Player>().HasIndex(p => p.PlayerGuid).IsUnique();
            modelBuilder.Entity<Player>().HasIndex(p => p.ConnectionId);

            CardSeeder.Seed(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).ModifiedAt = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

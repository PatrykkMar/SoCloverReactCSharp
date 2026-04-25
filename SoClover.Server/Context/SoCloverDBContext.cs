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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameRoom>()
                .HasMany(g => g.Players)
                .WithOne(p => p.GameRoom)
                .HasForeignKey(p => p.GameRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameRoom>()
                .HasOne(g => g.ActivePlayer)
                .WithMany()
                .HasForeignKey(g => g.ActivePlayerId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasOne(s => s.Card)
                .WithMany()
                .HasForeignKey(s => s.CardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GameRoom>()
                .Property(g => g.RoomCode);

            modelBuilder.Entity<Player>();
        }
    }
}

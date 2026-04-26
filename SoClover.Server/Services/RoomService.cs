using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Models;

namespace SoClover.Server.Services
{
    public interface IRoomService
    {
        Task<GameRoom> CreateRoomAsync();
        Task<Player> JoinRoomAsync(string roomCode, string playerName, string connectionId);
        Task<int?> LeaveRoomAsync(string connectionId);
    }

    public class RoomService(SoCloverDBContext context) : IRoomService
    {
        private readonly SoCloverDBContext _context = context;
        private static readonly Random _random = new();

        public async Task<GameRoom> CreateRoomAsync()
        {
            var room = new GameRoom
            {
                RoomCode = GenerateRoomCode(),
                Status = GameStatus.Lobby
            };

            _context.GameRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Player> JoinRoomAsync(string roomCode, string playerName, string connectionId)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.RoomCode == roomCode.ToUpper());

            if (room == null) throw new Exception("Room does not exist");
            if (room.Status != GameStatus.Lobby) throw new Exception("Game just started");
            if (room.Players.Count >= 6) throw new Exception("Room is full");

            var player = new Player
            {
                Name = playerName,
                ConnectionId = connectionId,
                GameRoomId = room.Id,
                IsReady = false,
                Score = 0
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<int?> LeaveRoomAsync(string connectionId)
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId);

            if (player == null) return null;

            int roomId = player.GameRoomId;
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            var anyLeft = await _context.Players.AnyAsync(p => p.GameRoomId == roomId);
            if (!anyLeft)
            {
                var room = await _context.GameRooms.FindAsync(roomId);
                if (room != null) _context.GameRooms.Remove(room);
                await _context.SaveChangesAsync();
                return null;
            }

            return roomId;
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string code;
            do
            {
                code = new string(Enumerable.Repeat(chars, 4)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            }
            while (_context.GameRooms.Any(r => r.RoomCode == code));

            return code;
        }
    }
}
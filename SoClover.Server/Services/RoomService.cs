using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Server.Models.DTOs;

namespace SoClover.Server.Services
{
    public interface IRoomService
    {
        Task<GameRoom> CreateRoomAsync();
        Task<RoomDataDTO> JoinRoomAsync(string roomCode, string playerName, Guid playerGuid);
        Task<RoomDataDTO?> LeaveRoomAsync(Guid playerGuid);
        Task<RoomDataDTO> GetRoomDataAsync(int roomId);
    }

    public class RoomService(SoCloverDBContext context, ICardManager cardManager) : IRoomService
    {
        private readonly SoCloverDBContext _context = context;
        private readonly ICardManager _cardManager = cardManager;
        private static readonly Random _random = new();

        public async Task<GameRoom> CreateRoomAsync()
        {
            var room = new GameRoom
            {
                RoomCode = GenerateRoomCode(),
                Status = GameStatus.Lobby
            };
            _context.GameRooms.Add(room);
            await _cardManager.CreateDeckForRoomAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<RoomDataDTO> JoinRoomAsync(string roomCode, string playerName, Guid playerGuid)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.RoomCode == roomCode.ToUpper());

            if (room == null) throw new Exception("Room does not exist");

            var existingPlayer = _context.Players.FirstOrDefault(p => p.PlayerGuid == playerGuid);

            if (existingPlayer != null)
            {
                await _context.SaveChangesAsync();
                return await GetRoomDataAsync(room.Id);
            }

            if (room.Status != GameStatus.Lobby) throw new Exception("Game just started"); //TODO: Possibility to join a game in progress
            if (room.Players.Count >= 6) throw new Exception("Room is full");

            var player = new Player
            {
                Name = playerName,
                GameRoom = room,
                PlayerGuid = playerGuid,
                IsReady = false,
                Score = 0
            };

            room.Players.Add(player);

            await _context.SaveChangesAsync();
            return await GetRoomDataAsync(room.Id);
        }

        public async Task<RoomDataDTO?> LeaveRoomAsync(Guid playerGuid)
        {
            var player = await _context.Players
                .Include(p => p.GameRoom)
                    .ThenInclude(r => r.Players)
                .FirstOrDefaultAsync(p => p.PlayerGuid == playerGuid);

            if (player == null) return null;

            var room = player.GameRoom;
            string roomCode = room.RoomCode;
            int roomId = room.Id;

            bool anyLeft = room.Players.Any(p => p.Id != player.Id);

            if (!anyLeft)
                _context.GameRooms.Remove(room);
            else
                _context.Players.Remove(player);

            await _context.SaveChangesAsync();

            if (!anyLeft)
                return null;

            return await GetRoomDataAsync(roomId);
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

        public async Task<RoomDataDTO> GetRoomDataAsync(int roomId)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null) throw new Exception("Room not found");

            return new RoomDataDTO(room);
        }
    }
}
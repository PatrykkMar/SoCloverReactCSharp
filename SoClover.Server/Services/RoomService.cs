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
        Task<RoomDataDTO> JoinRoomAsync(string roomCode, string playerName, string connectionId, Guid playerGuid);
        Task<RoomDataDTO?> LeaveRoomAsync(string connectionId);
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

            var cards = await _context.Cards.ToListAsync();

            var deck = cards.Select(c => new GameRoomCard
            {
                CardId = c.Id,
                GameRoom = room
            }).ToList();

            room.GameRoomCards = deck;

            _context.GameRooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<RoomDataDTO> JoinRoomAsync(string roomCode, string playerName, string connectionId, Guid playerGuid)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.RoomCode == roomCode.ToUpper());

            if (room == null) throw new Exception("Room does not exist");
            if (room.Status != GameStatus.Lobby) throw new Exception("Game just started"); //TODO: Possibility to join a game in progress
            if (room.Players.Count >= 6) throw new Exception("Room is full");

            var player = new Player
            {
                Name = playerName,
                ConnectionId = connectionId,
                GameRoomId = room.Id,
                PlayerGuid = playerGuid,
                IsReady = false,
                Score = 0
            };

            room.Players.Add(player);

            var roomData = new RoomDataDTO(room);

            await _context.SaveChangesAsync();
            return roomData;
        }

        public async Task<RoomDataDTO?> LeaveRoomAsync(string connectionId)
        {
            var player = await _context.Players
                .Include(p => p.GameRoom)
                    .ThenInclude(r => r.Players)
                .FirstOrDefaultAsync(p => p.ConnectionId == connectionId);

            if (player == null) return null;

            var room = player.GameRoom;
            var roomId = room.Id;

            _context.Players.Remove(player);

            bool anyLeft = room.Players.Any(p => p.Id != player.Id);

            if (!anyLeft)
            {
                _context.GameRooms.Remove(room);
            }

            await _context.SaveChangesAsync();

            var roomData = new RoomDataDTO(room);
            return roomData;
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
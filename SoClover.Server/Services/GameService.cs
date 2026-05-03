using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Server.Models.DTOs;

namespace SoClover.Server.Services
{
    public interface IGameService
    {
        public Task<GameRoom> StartGameAsync(string roomCode);
        Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoom(int[] playersId);
    }

    public class GameService(SoCloverDBContext context, ICardManager cardManager) : IGameService
    {
        private readonly SoCloverDBContext _context = context;
        private readonly ICardManager _cardManager = cardManager;

        public async Task<GameRoom> StartGameAsync(string playerConnectionId)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.ConnectionId == playerConnectionId));

            if (room == null) throw new Exception("Room does not exist");
            if (room.Status != GameStatus.Lobby) throw new Exception("Game already started");
            if (room.Players.Count < 2) throw new Exception("Not enough players to start");
            room.Status = GameStatus.Writing;


            await _cardManager.AssignCardsToPlayersAsync([.. room.Players.Select(x => x.Id)], room.Id);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoom(int[] playersId)
        {
            var dict = new Dictionary<Player, BoardDataDTO>();
            var players = await _context.Players
                .Include(p => p.Board)
                .ThenInclude(b => b.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .ThenInclude(grc => grc.Card)
                .Where(p => playersId.Contains(p.Id))
                .ToListAsync();

            dict = players.ToDictionary(
                p => p,
                p => new BoardDataDTO(p.Board)
            );
            return dict;
        }
    }
}

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
        Task<GameRoom> SubmitCluesAsync(string playerConnectionId, string[] words);
        Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoom(int roomId);
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


        public async Task<GameRoom> SubmitCluesAsync(string playerConnectionId, string[] words)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .ThenInclude(p => p.Board)
                .ThenInclude(b => b.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.ConnectionId == playerConnectionId));

            if (room == null) throw new Exception("Room not found");

            room.Status = GameStatus.Solving;
            var currentPlayer = room.Players.First(p => p.ConnectionId == playerConnectionId);
            var board = currentPlayer.Board;

            if (board == null) throw new Exception("Player board not found");

            board.IsActive = true;
            room.ActivePlayer = currentPlayer;

            if (words.Length >= 4)
            {
                board.TopClue = words[0];
                board.RightClue = words[1];
                board.BottomClue = words[2];
                board.LeftClue = words[3];
            }
            else
            {
                throw new Exception("Not enough clues provided");
            }

            foreach (var bs in board.BoardSlots)
            {
                if (bs.GameRoomCard == null) throw new Exception("GameRoomCard not found");
                bs.GameRoomCard.Location = CardLocation.InHand;
                bs.TargetGameRoomCard = bs.GameRoomCard;
                bs.TargetRotation = bs.GameRoomCard?.CurrentRotation;
                bs.GameRoomCard = null;
            }

            var twoAdditionalCards = await _context.GameRoomCards
                .Where(grc => grc.GameRoomId == room.Id && grc.Location == CardLocation.InDeck)
                .Take(2)
                .ToListAsync();

            foreach (var grc in twoAdditionalCards)
                grc.Location = CardLocation.InHand;

            currentPlayer.IsReady = true;

            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoom(int roomId)
        {
            var dict = new Dictionary<Player, BoardDataDTO>();

            var room = await _context.GameRooms
                .Include(r => r.ActivePlayer)
                .Include(r => r.Players)
                .ThenInclude(p => p.Board)
                .ThenInclude(b => b.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .ThenInclude(grc => grc.Card)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if(room == null) throw new Exception($"Room {roomId} not found");

            var players = room.Players;


            if(room.Status == GameStatus.Solving)
            {
                var checkedPlayer = players.FirstOrDefault(p => p.Id == room.CheckedPlayerId) ?? throw new Exception("Active player not found in a room");
                if (checkedPlayer.Board == null) throw new Exception("Active player board not found");

                dict = players.ToDictionary(
                    p => p,
                    p => new BoardDataDTO(checkedPlayer.Board, p.Id == checkedPlayer.Id)
                );
                return dict;
            }

            dict = players.ToDictionary(
                p => p,
                p => new BoardDataDTO(p.Board)
            );
            return dict;
        }
    }
}

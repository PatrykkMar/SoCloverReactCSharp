using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;

namespace SoClover.Server.Services
{
    public interface IGameFlowService
    {
        Task<GameRoom> StartGameAsync(Guid playerGuid);
        Task<GameRoom> SubmitCluesAsync(Guid playerGuid, string[] words);
        Task<GameRoom> ReturnToWritingAsync(Guid playerGuid);
    }

    public class GameFlowService(SoCloverDBContext context, ICardManager cardManager) : IGameFlowService
    {
        private readonly SoCloverDBContext _context = context;
        private readonly ICardManager _cardManager = cardManager;

        public async Task<GameRoom> StartGameAsync(Guid playerGuid)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid))
                ?? throw new Exception("Room does not exist");

            if (room.Status != GameStatus.Lobby) throw new Exception("Game already started");
            if (room.Players.Count < 2) throw new Exception("Not enough players to start");

            room.Status = GameStatus.Writing;

            await _cardManager.AssignCardsToPlayersAsync([.. room.Players.Select(x => x.Id)], room.Id);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<GameRoom> SubmitCluesAsync(Guid playerGuid, string[] words)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .ThenInclude(p => p.Board)
                .ThenInclude(b => b!.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid))
                ?? throw new Exception("Room not found");

            room.Status = GameStatus.Solving;

            var currentPlayer = room.Players.First(p => p.PlayerGuid == playerGuid);
            var board = currentPlayer.Board ?? throw new Exception("Player board not found");

            board.IsActive = true;
            room.CheckedPlayer = currentPlayer;

            if (words.Length < 4) throw new Exception("Not enough clues provided");

            board.TopClue = words[0];
            board.RightClue = words[1];
            board.BottomClue = words[2];
            board.LeftClue = words[3];

            var ran = new Random();
            foreach (var bs in board.BoardSlots)
            {
                if (bs.GameRoomCard == null) throw new Exception("GameRoomCard not found");
                bs.GameRoomCard.Location = CardLocation.InHand;
                bs.TargetGameRoomCard = bs.GameRoomCard;
                bs.TargetRotation = bs.GameRoomCard.CurrentRotation;
                bs.GameRoomCard.CurrentRotation = ran.Next(0, 4);
                bs.GameRoomCard = null;
            }

            var twoAdditionalCards = await _context.GameRoomCards
                .Where(grc => grc.GameRoomId == room.Id && grc.Location == CardLocation.InDeck)
                .OrderBy(x => Guid.NewGuid())
                .Take(2)
                .ToListAsync();

            foreach (var grc in twoAdditionalCards)
                grc.Location = CardLocation.InHand;

            currentPlayer.IsReady = true;

            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<GameRoom> ReturnToWritingAsync(Guid playerGuid)
        {
            var room = await _context.GameRooms
                .Include(r => r.CheckedPlayer)
                .Include(r => r.Players)
                    .ThenInclude(p => p.Board)
                        .ThenInclude(b => b!.BoardSlots)
                            .ThenInclude(bs => bs.GameRoomCard)
                .Include(r => r.GameRoomCards)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid))
                ?? throw new Exception("Room not found");

            if (room.Status != GameStatus.Solving) throw new Exception("Can only rollback from Solving state");

            var checkedPlayer = room.CheckedPlayer;
            if (checkedPlayer?.Board == null) throw new Exception("No active checked player or board found to rollback");

            var board = checkedPlayer.Board;

            foreach (var slot in board.BoardSlots)
            {
                if (slot.GameRoomCard != null)
                {
                    slot.GameRoomCard.Location = CardLocation.InDeck;
                    slot.GameRoomCard.CurrentRotation = 0;
                    slot.GameRoomCard = null;
                }

                slot.TargetGameRoomCard = null;
                slot.TargetGameRoomCardId = null;
                slot.TargetRotation = 0;
                slot.IsCorrect = false;
            }

            board.TopClue = "";
            board.RightClue = "";
            board.BottomClue = "";
            board.LeftClue = "";
            board.IsActive = false;

            var cardsInHand = room.GameRoomCards.Where(grc => grc.Location == CardLocation.InHand);
            foreach (var card in cardsInHand)
            {
                card.Location = CardLocation.InDeck;
                card.CurrentRotation = 0;
            }

            foreach (var p in room.Players) p.IsReady = false;

            room.Status = GameStatus.Writing;
            room.CheckedPlayer = null;

            await _cardManager.AssignCardsToPlayersAsync([checkedPlayer.Id], room.Id);
            await _context.SaveChangesAsync();
            return room;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Server.Models.DTOs;

namespace SoClover.Server.Services
{
    public interface IGameService
    {
        public Task<GameRoom> StartGameAsync(Guid playerGuid);
        Task<GameRoom> SubmitCluesAsync(Guid playerGuid, string[] words);
        Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoomAsync(int roomId);
        Task<GameRoom> RotateCardAsync(int gameRoomCardId);
        Task<GameRoom> MoveCardToSlotAsync(Guid playerGuid, int gameRoomCardId, int positionIndex);
        Task<GameRoom> CheckAsync(Guid playerGuid);
        Task<GameRoom> ReturnToWritingAsync(Guid playerGuid);
    }

    public class GameService(SoCloverDBContext context, ICardManager cardManager) : IGameService
    {
        private readonly SoCloverDBContext _context = context;
        private readonly ICardManager _cardManager = cardManager;

        public async Task<GameRoom> StartGameAsync(Guid playerGuid)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid));
            if (room == null) throw new Exception("Room does not exist");
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
                .ThenInclude(b => b.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid));

            if (room == null) throw new Exception("Room not found");

            room.Status = GameStatus.Solving;

            var currentPlayer = room.Players.First(p => p.PlayerGuid == playerGuid);
            var board = currentPlayer.Board;

            if (board == null) throw new Exception("Player board not found");

            board.IsActive = true;
            room.CheckedPlayer = currentPlayer;

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

        public async Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoomAsync(int roomId)
        {
            var dict = new Dictionary<Player, BoardDataDTO>();

            var room = await _context.GameRooms
                .Include(r => r.CheckedPlayer)
                .Include(r => r.Players)
                .ThenInclude(p => p.Board)
                .ThenInclude(b => b.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .ThenInclude(grc => grc.Card)
                .Include(r => r.GameRoomCards)
                .ThenInclude(grc => grc.Card)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if(room == null) throw new Exception($"Room {roomId} not found");

            var players = room.Players;


            if(room.Status == GameStatus.Solving)
            {
                var checkedPlayer = room.CheckedPlayer;
                if (checkedPlayer?.Board == null) throw new Exception("Active player board not found");

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

        public async Task<GameRoom> RotateCardAsync(int gameRoomCardId)
        {
            var card = await _context
                .GameRoomCards
                .Include(grc => grc.GameRoom)
                .FirstOrDefaultAsync(grc => grc.Id == gameRoomCardId) ?? throw new Exception("Card not found");
            card.CurrentRotation = (card.CurrentRotation + 1) % 4;
            await _context.SaveChangesAsync();
            return card.GameRoom;
        }

        public async Task<GameRoom> MoveCardToSlotAsync(Guid playerGuid, int gameRoomCardId, int positionIndex)
        {
            var room = await _context.GameRooms
                .Include(r => r.Players)
                .ThenInclude(p => p.Board)
                .ThenInclude(b => b!.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid));

            if (room == null) throw new Exception("Connection not found");

            var player = room.Players.First(p => p.PlayerGuid == playerGuid);
            var board = room.Players.Select(p => p.Board).FirstOrDefault(b => b!.IsActive) ?? throw new Exception("Player does not have an assigned board.");

            var movingCard = await _context.GameRoomCards
                .FirstOrDefaultAsync(grc => grc.Id == gameRoomCardId && grc.GameRoomId == room.Id);

            if (movingCard == null) throw new Exception("Card does not belong to this game room.");

            var targetSlot = board.BoardSlots.FirstOrDefault(bs => bs.PositionIndex == positionIndex);
            if (targetSlot == null) throw new Exception("Board slot not found");
            if (targetSlot.IsCorrect) throw new Exception("Target slot is correct, therefore it can't be moved");

            if (targetSlot.GameRoomCard != null)
            {
                var oldCard = targetSlot.GameRoomCard;

                if (oldCard.Id == movingCard.Id) return room;

                oldCard.Location = CardLocation.InHand;
                targetSlot.GameRoomCard = null;
            }

            targetSlot.GameRoomCard = movingCard;
            movingCard.Location = CardLocation.OnBoard;

            await _context.SaveChangesAsync();
            return room;
        }
        public async Task<GameRoom> CheckAsync(Guid playerGuid)
        {
            var board = _context.Boards
                .Include(b => b.Player)
                .ThenInclude(p => p.GameRoom)
                .ThenInclude(gr => gr.Players)
                .Include(b => b.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .FirstOrDefault(x => x.IsActive && x.Player.GameRoom.Players.Any(x=>x.PlayerGuid == playerGuid)) ?? throw new Exception("Active board for player not found");

            foreach(var slot in board.BoardSlots)
            {
                if(slot.GameRoomCard == null) throw new Exception("Not all slots are filled");
                slot.IsCorrect = slot.TargetGameRoomCardId == slot.GameRoomCard.Id && slot.TargetRotation == slot.GameRoomCard.CurrentRotation;
            }

            await _context.SaveChangesAsync();
            return board.Player.GameRoom;
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
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid));

            if (room == null) throw new Exception("Room not found");
            if (room.Status != GameStatus.Solving) throw new Exception("Can only rollback from Solving state");

            var checkedPlayer = room.CheckedPlayer;
            if (checkedPlayer == null || checkedPlayer.Board == null)
                throw new Exception("No active checked player or board found to rollback");

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

            foreach (var p in room.Players)
            {
                p.IsReady = false;
            }

            room.Status = GameStatus.Writing;
            room.CheckedPlayer = null;

            await _cardManager.AssignCardsToPlayersAsync([checkedPlayer.Id], room.Id);
            await _context.SaveChangesAsync();
            return room;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Models;
using SoClover.Server.Models.DTOs;

namespace SoClover.Server.Services
{
    public interface IBoardService
    {
        Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoomAsync(int roomId);
        Task<GameRoom> RotateCardAsync(int gameRoomCardId);
        Task<GameRoom> MoveCardToSlotAsync(Guid playerGuid, int gameRoomCardId, int positionIndex);
        Task<GameRoom> CheckAsync(Guid playerGuid);
    }

    public class BoardService(SoCloverDBContext context) : IBoardService
    {
        private readonly SoCloverDBContext _context = context;

        public async Task<Dictionary<Player, BoardDataDTO>> CreateBoardDTOsForPlayersInRoomAsync(int roomId)
        {
            var room = await _context.GameRooms
                .Include(r => r.CheckedPlayer)
                .Include(r => r.Players)
                    .ThenInclude(p => p.Board)
                    .ThenInclude(b => b!.BoardSlots)
                    .ThenInclude(bs => bs.GameRoomCard)
                    .ThenInclude(grc => grc!.Card)
                .Include(r => r.GameRoomCards)
                    .ThenInclude(grc => grc.Card)
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.Id == roomId)
                ?? throw new Exception($"Room {roomId} not found");

            if (room.Status == GameStatus.Solving)
            {
                var checkedPlayer = room.CheckedPlayer;
                if (checkedPlayer?.Board == null) throw new Exception("Active player board not found");

                return room.Players.ToDictionary(
                    p => p,
                    p => new BoardDataDTO(checkedPlayer.Board, p.Id == checkedPlayer.Id)
                );
            }

            return room.Players.ToDictionary(
                p => p,
                p => new BoardDataDTO(p.Board!)
            );
        }

        public async Task<GameRoom> RotateCardAsync(int gameRoomCardId)
        {
            var card = await _context.GameRoomCards
                .Include(grc => grc.GameRoom)
                .FirstOrDefaultAsync(grc => grc.Id == gameRoomCardId)
                ?? throw new Exception("Card not found");

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
                .FirstOrDefaultAsync(r => r.Players.Any(p => p.PlayerGuid == playerGuid))
                ?? throw new Exception("Room not found");

            var board = room.Players.Select(p => p.Board).FirstOrDefault(b => b!.IsActive)
                ?? throw new Exception("Player does not have an assigned board.");

            var movingCard = await _context.GameRoomCards
                .FirstOrDefaultAsync(grc => grc.Id == gameRoomCardId && grc.GameRoomId == room.Id)
                ?? throw new Exception("Card does not belong to this game room.");

            var targetSlot = board.BoardSlots.FirstOrDefault(bs => bs.PositionIndex == positionIndex)
                ?? throw new Exception("Board slot not found");

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
            var board = await _context.Boards
                .Include(b => b.Player)
                .ThenInclude(p => p.GameRoom)
                .ThenInclude(gr => gr.Players)
                .Include(b => b.BoardSlots)
                .ThenInclude(bs => bs.GameRoomCard)
                .FirstOrDefaultAsync(x => x.IsActive && x.Player.GameRoom.Players.Any(p => p.PlayerGuid == playerGuid))
                ?? throw new Exception("Active board for player not found");

            board.Player.GameRoom.NumberOfAttempts++;

            foreach (var slot in board.BoardSlots)
            {
                if (slot.GameRoomCard == null) throw new Exception("Not all slots are filled");
                slot.IsCorrect = slot.TargetGameRoomCardId == slot.GameRoomCard.Id && slot.TargetRotation == slot.GameRoomCard.CurrentRotation;
            }

            await _context.SaveChangesAsync();
            return board.Player.GameRoom;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Models;

namespace SoClover.Server.Helpers
{
    public interface ICardManager
    {
        Task CreateDeckAsync(int roomId);
        Task AssignCardsToPlayersAsync(int[] playerIds, int roomId);
        Task MoveToBoardAsync(int gameRoomCardId, int boardId, int position);
        Task MoveToHandAsync(int gameRoomCardId);
    }
    public class CardManager : ICardManager
    {
        private readonly SoCloverDBContext _context;

        public CardManager(SoCloverDBContext context)
        {
            _context = context;
        }

        public async Task CreateDeckAsync(int roomId)
        {
            var allCards = await _context.Cards.ToListAsync();
            var deck = allCards.Select(c => new GameRoomCard
            {
                GameRoomId = roomId,
                CardId = c.Id,
                Location = CardLocation.InDeck
            })
            .OrderBy(r => Guid.NewGuid());

            _context.GameRoomCards.AddRange(deck);
            await _context.SaveChangesAsync();
        }

        public async Task AssignCardsToPlayersAsync(int[] playerIds, int roomId)
        {
            var players = await _context.Players.Where(p => playerIds.Contains(p.Id)).ToListAsync();

            foreach (var player in players)
            {
                var drawnCards = await _context.GameRoomCards
                    .Where(c => c.GameRoomId == roomId && c.Location == CardLocation.InDeck)
                    .Take(4)
                    .ToListAsync();

                var board = new Board { PlayerId = player.Id };
                _context.Boards.Add(board);
                await _context.SaveChangesAsync();

                for (int i = 0; i < drawnCards.Count; i++)
                {
                    drawnCards[i].Location = CardLocation.OnBoard;

                    _context.BoardSlots.Add(new BoardSlot
                    {
                        BoardId = board.Id,
                        GameRoomCardId = drawnCards[i].Id,
                        PositionIndex = i,
                        CurrentRotation = 0
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task MoveToBoardAsync(int gameRoomCardId, int boardId, int position)
        {
            var card = await _context.GameRoomCards.FindAsync(gameRoomCardId);
            if (card == null) return;

            card.Location = CardLocation.OnBoard;

            var existingSlot = await _context.BoardSlots
                .FirstOrDefaultAsync(s => s.BoardId == boardId && s.PositionIndex == position);

            if (existingSlot != null)
            {
                if (existingSlot.GameRoomCardId.HasValue)
                {
                    var oldCard = await _context.GameRoomCards.FindAsync(existingSlot.GameRoomCardId.Value);
                    if (oldCard != null) oldCard.Location = CardLocation.InHand;
                }

                existingSlot.GameRoomCardId = gameRoomCardId;
                existingSlot.CurrentRotation = 0;
            }
            else
            {
                _context.BoardSlots.Add(new BoardSlot
                {
                    BoardId = boardId,
                    GameRoomCardId = gameRoomCardId,
                    PositionIndex = position,
                    CurrentRotation = 0
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task MoveToHandAsync(int gameRoomCardId)
        {
            var card = await _context.GameRoomCards.FindAsync(gameRoomCardId);
            if (card == null) return;

            card.Location = CardLocation.InHand;

            var slot = await _context.BoardSlots.FirstOrDefaultAsync(s => s.GameRoomCardId == gameRoomCardId);
            if (slot != null)
            {
                slot.GameRoomCard = null;
            }

            await _context.SaveChangesAsync();
        }
    }
}

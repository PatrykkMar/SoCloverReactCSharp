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

        public async Task<List<GameRoomCard>> DrawCardsFromDeckAsync(int roomId, int count)
        {
            var stats = await _context.GameRoomCards
                .Where(c => c.GameRoomId == roomId)
                .GroupBy(c => 1)
                .Select(g => new {
                    InDeck = g.Count(c => c.Location == CardLocation.InDeck),
                    Discarded = g.Count(c => c.Location == CardLocation.Discarded)
                })
                .FirstOrDefaultAsync();

            if (stats == null || (stats.InDeck + stats.Discarded) < count)
            {
                throw new InvalidOperationException("Not enough cards in the deck to draw.");
            }

            if (stats.InDeck < count)
            {
                var discardedCards = await _context.GameRoomCards
                    .Where(c => c.GameRoomId == roomId && c.Location == CardLocation.Discarded)
                    .ToListAsync();

                foreach (var card in discardedCards)
                {
                    card.Location = CardLocation.InDeck;
                }

                await _context.SaveChangesAsync();
            }

            return await _context.GameRoomCards
                .Where(c => c.GameRoomId == roomId && c.Location == CardLocation.InDeck)
                .OrderBy(r => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
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
            var allDrawnCards = await DrawCardsFromDeckAsync(roomId, 4 * playerIds.Length);
            var cardsQueue = new Queue<GameRoomCard>(allDrawnCards);

            foreach (var player in players)
            {
                var board = new Board { PlayerId = player.Id };

                for (int i = 0; i < 4; i++)
                {
                    if (!cardsQueue.TryDequeue(out var card)) break;

                    card.Location = CardLocation.OnBoard;
                    card.CurrentRotation = new Random().Next(0, 4);

                    board.BoardSlots.Add(new BoardSlot
                    {
                        GameRoomCard = card, PositionIndex = i
                    });
                }
                _context.Boards.Add(board);
            }

            await _context.SaveChangesAsync();
        }

        public async Task MoveToBoardAsync(int gameRoomCardId, int boardId, int position)
        {
            var card = await _context.GameRoomCards.FindAsync(gameRoomCardId);
            if (card == null) return;

            card.Location = CardLocation.OnBoard;

            var existingSlot = await _context.BoardSlots
                .FirstOrDefaultAsync(s => s.BoardId == boardId);

            if (existingSlot != null)
            {
                if (existingSlot.GameRoomCardId.HasValue)
                {
                    var oldCard = await _context.GameRoomCards.FindAsync(existingSlot.GameRoomCardId.Value);
                    if (oldCard != null) oldCard.Location = CardLocation.InHand;
                }

                existingSlot.GameRoomCardId = gameRoomCardId;
            }
            else
            {
                _context.BoardSlots.Add(new BoardSlot
                {
                    BoardId = boardId,
                    GameRoomCardId = gameRoomCardId
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

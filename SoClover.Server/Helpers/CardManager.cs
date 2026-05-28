using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Models;

namespace SoClover.Server.Helpers
{
    public interface ICardManager
    {
        Task CreateDeckForRoomAsync(GameRoom room);
        Task AssignCardsToPlayersAsync(int[] playerIds, int roomId);
        Task<List<GameRoomCard>> DrawBonusCardsAsync(int roomId, int count);
        Task ReleasePlayerCardsToDeckAsync(int roomId, int playerId);
    }
    public class CardManager : ICardManager
    {
        private readonly SoCloverDBContext _context;
        public const int CARDS_NEEDED_FOR_ROOM = 40;

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

        public async Task CreateDeckForRoomAsync(GameRoom room)
        {
            var allCards = await _context.Cards.ToListAsync();
            var deck = allCards.Select(c => new GameRoomCard
            {
                GameRoom = room,
                CardId = c.Id,
                Location = CardLocation.InDeck
            })
            .OrderBy(r => Guid.NewGuid());

            _context.GameRoomCards.AddRange(deck);
        }

        public async Task AssignCardsToPlayersAsync(int[] playerIds, int roomId)
        {
            var players = await _context.Players.Where(p => playerIds.Contains(p.Id)).ToListAsync();
            var allDrawnCards = await DrawCardsFromDeckAsync(roomId, 4 * playerIds.Length);
            var cardsQueue = new Queue<GameRoomCard>(allDrawnCards);

            var random = new Random();

            foreach (var player in players)
            {
                var board = new Board { Player = player };

                for (int i = 0; i < 4; i++)
                {
                    if (!cardsQueue.TryDequeue(out var card)) break;

                    card.Location = CardLocation.OnBoard;
                    card.CurrentRotation = random.Next(0, 4);

                    board.BoardSlots.Add(new BoardSlot
                    {
                        GameRoomCard = card,
                        TargetGameRoomCard = card,
                        PositionIndex = i
                    });
                }
                _context.Boards.Add(board);
            }
        }

        public async Task<List<GameRoomCard>> DrawBonusCardsAsync(int roomId, int count)
        {
            var bonusCards = await DrawCardsFromDeckAsync(roomId, count);

            foreach (var card in bonusCards)
            {
                card.Location = CardLocation.InHand;
            }

            return bonusCards;
        }

        public async Task ReleasePlayerCardsToDeckAsync(int roomId, int playerId)
        {
            var boardSlots = await _context.BoardSlots
                .Where(bs => bs.Board!.PlayerId == playerId)
                .Include(bs => bs.GameRoomCard)
                .ToListAsync();

            foreach (var slot in boardSlots)
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

            var bonusCards = await _context.GameRoomCards
                .Where(grc => grc.GameRoomId == roomId && grc.Location == CardLocation.InHand)
                .ToListAsync();

            foreach (var card in bonusCards)
            {
                card.Location = CardLocation.InDeck;
                card.CurrentRotation = 0;
            }
        }

    }
}

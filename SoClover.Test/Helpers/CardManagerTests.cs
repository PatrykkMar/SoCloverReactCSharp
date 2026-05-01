using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Tests.Context;

namespace SoClover.Tests.Helpers
{
    public class CardManagerTests
    {
        [Fact]
        public async Task CreateDeckAsync_ShouldCopyAllCardsToRoom()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var cardManager = new CardManager(context);

            // Act
            await cardManager.CreateDeckAsync(roomId: 1);

            // Assert
            var roomCards = await context.GameRoomCards.Where(rc => rc.GameRoomId == 1).ToListAsync();
            var allCards = await context.Cards.ToListAsync();
            roomCards.Should().HaveCount(allCards.Count);
            roomCards.All(rc => rc.Location == CardLocation.InDeck).Should().BeTrue();
        }

        [Fact]
        public async Task AssignCardsToPlayersAsync_ShouldGiveEachPlayerUniqueCards()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var cardManager = new CardManager(context);


            context.GameRooms.Add(new GameRoom { Id = 1, RoomCode = "TEST" });

            context.Players.Add(new Player { Name = "Gracz 1", GameRoomId = 1 });
            context.Players.Add(new Player { Name = "Gracz 2", GameRoomId = 1 });
            await context.SaveChangesAsync();

            await cardManager.CreateDeckAsync(roomId: 1);

            // Act


            var players = await context.Players.Where(p => p.GameRoomId == 1).ToListAsync();
            await cardManager.AssignCardsToPlayersAsync(players.Select(x => x.Id).ToArray(), roomId: 1);

            // Assert
            var boards = await context.Boards.Include(b => b.BoardSlots).ToListAsync();
            boards.Should().HaveCount(2);

            boards.All(b => b.BoardSlots.Count == 4).Should().BeTrue();

            var assignedCards = await context.GameRoomCards.Where(c => c.Location == CardLocation.OnBoard).ToListAsync();
            assignedCards.Should().HaveCount(8);
        }

        [Fact]
        public async Task MoveToBoardAsync_WhenSlotIsOccupied_ShouldMoveOldCardToHand()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var cardManager = new CardManager(context);

            var cardInHand = new GameRoomCard { Id = 1, GameRoomId = 1, Location = CardLocation.InHand };
            var cardOnBoard = new GameRoomCard { Id = 2, GameRoomId = 1, Location = CardLocation.OnBoard };

            context.GameRoomCards.AddRange(cardInHand, cardOnBoard);

            var slot = new BoardSlot
            {
                BoardId = 50,
                GameRoomCardId = 2,
                PositionIndex = 0
            };
            context.BoardSlots.Add(slot);
            await context.SaveChangesAsync();

            // Act
            await cardManager.MoveToBoardAsync(gameRoomCardId: 1, boardId: 50, position: 0);

            // Assert
            var updatedCard1 = await context.GameRoomCards.FindAsync(1);
            var updatedCard2 = await context.GameRoomCards.FindAsync(2);

            updatedCard1.Location.Should().Be(CardLocation.OnBoard);
            updatedCard2.Location.Should().Be(CardLocation.InHand);

            var currentSlot = await context.BoardSlots.FirstAsync(s => s.BoardId == 50 && s.PositionIndex == 0);
            currentSlot.GameRoomCardId.Should().Be(1);
        }

        [Fact]
        public async Task DrawCards_ShouldShuffleDiscardIntoDeck_WhenDeckIsTooSmall()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var cardManager = new CardManager(context);

            int roomId = 1;

            for (int i = 0; i < 10; i++)
            {
                context.GameRoomCards.Add(new GameRoomCard
                {
                    GameRoomId = roomId,
                    CardId = i,
                    Location = CardLocation.InDeck
                });
            }
            await context.SaveChangesAsync();

            var cardsToDiscard = await context.GameRoomCards.Take(9).ToListAsync();
            foreach (var card in cardsToDiscard)
            {
                card.Location = CardLocation.Discarded;
            }
            await context.SaveChangesAsync();

            // Act
            var drawnCards = await cardManager.DrawCardsFromDeckAsync(roomId, 4);
            drawnCards.ForEach(x => x.Location = CardLocation.InHand);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(4, drawnCards.Count);

            var deckCount = await context.GameRoomCards
                .CountAsync(c => c.GameRoomId == roomId && c.Location == CardLocation.InDeck);

            Assert.Equal(6, deckCount);
            Assert.All(drawnCards, c => Assert.Equal(CardLocation.InHand, c.Location));
        }

        [Fact]
        public async Task MoveToHandAsync_ShouldUpdateLocationAndClearSlot()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var cardManager = new CardManager(context);

            var card = new GameRoomCard
            {
                Id = 100,
                Location = CardLocation.OnBoard,
                GameRoomId = 1
            };

            var slot = new BoardSlot
            {
                Id = 1,
                GameRoomCardId = 100,
                PositionIndex = 0
            };

            context.GameRoomCards.Add(card);
            context.BoardSlots.Add(slot);
            await context.SaveChangesAsync();

            // Act
            await cardManager.MoveToHandAsync(100);

            // Assert
            var updatedCard = await context.GameRoomCards.FindAsync(100);
            var updatedSlot = await context.BoardSlots.FindAsync(1);

            Assert.Equal(CardLocation.InHand, updatedCard.Location);

        }
    }
}
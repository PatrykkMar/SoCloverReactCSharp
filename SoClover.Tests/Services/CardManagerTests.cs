using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Tests.Context;

namespace SoClover.Tests.Services
{
    public class CardManagerTests
    {
        [Fact]
        public async Task CreateDeckAsync_ShouldCopyAllCardsToRoom()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var cardManager = new CardManager(context);

            context.Cards.AddRange(new List<Card> {
                new Card { Id = 1, WordTop = "A", WordRight = "B", WordBottom = "C", WordLeft = "D" },
                new Card { Id = 2, WordTop = "E", WordRight = "F", WordBottom = "G", WordLeft = "H" }
            });
            await context.SaveChangesAsync();

            // Act
            await cardManager.CreateDeckAsync(roomId: 1);

            // Assert
            var roomCards = await context.GameRoomCards.Where(rc => rc.GameRoomId == 1).ToListAsync();
            roomCards.Should().HaveCount(2);
            roomCards.All(rc => rc.Location == CardLocation.InDeck).Should().BeTrue();
        }

        [Fact]
        public async Task AssignCardsToPlayersAsync_ShouldGiveEachPlayerUniqueCards()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var cardManager = new CardManager(context);

            for (int i = 1; i <= 8; i++)
                context.Cards.Add(new Card { Id = i, WordTop = "W" + i, WordRight = "W", WordBottom = "W", WordLeft = "W" });

            context.Players.Add(new Player { Id = 10, Name = "Gracz 1", GameRoomId = 1 });
            context.Players.Add(new Player { Id = 11, Name = "Gracz 2", GameRoomId = 1 });
            await context.SaveChangesAsync();

            await cardManager.CreateDeckAsync(roomId: 1);

            // Act
            await cardManager.AssignCardsToPlayersAsync(new[] { 10, 11 }, roomId: 1);

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
    }
}
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Tests.Context;

namespace SoClover.Tests.Helpers
{
    public class CardManagerTests : IDisposable
    {
        private readonly SoCloverDBContext _context;
        private readonly CardManager _cardManager;

        public CardManagerTests()
        {
            _context = TestDbContextFactory.Create();
            _cardManager = new CardManager(_context);
        }

        public void Dispose()
        {
            TestDbContextFactory.Destroy(_context);
        }

        #region CreateDeckForRoomAsync Tests

        [Fact]
        public async Task CreateDeckForRoomAsync_ShouldCopyAllCardsFromDatabaseToRoom()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "DECK" };
            _context.GameRooms.Add(room);
            await _context.SaveChangesAsync();

            var totalAvailableCards = await _context.Cards.CountAsync();

            // Act
            await _cardManager.CreateDeckForRoomAsync(room);
            await _context.SaveChangesAsync();

            // Assert
            var roomCards = await _context.GameRoomCards.Where(rc => rc.GameRoomId == room.Id).ToListAsync();

            roomCards.Should().HaveCount(totalAvailableCards);
            roomCards.All(rc => rc.Location == CardLocation.InDeck).Should().BeTrue();
        }

        #endregion

        #region AssignCardsToPlayersAsync Tests

        [Fact]
        public async Task AssignCardsToPlayersAsync_ShouldGiveEachPlayerFourCardsOnBoardWithTargetConfiguration()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "ASGN" };
            var p1 = new Player { Name = "Gracz 1", GameRoom = room };
            var p2 = new Player { Name = "Gracz 2", GameRoom = room };

            _context.GameRooms.Add(room);
            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            await _cardManager.CreateDeckForRoomAsync(room);
            await _context.SaveChangesAsync();

            int[] playerIds = [p1.Id, p2.Id];

            // Act
            await _cardManager.AssignCardsToPlayersAsync(playerIds, room.Id);
            await _context.SaveChangesAsync();

            // Assert
            var boards = await _context.Boards.Include(b => b.BoardSlots).ToListAsync();
            boards.Should().HaveCount(2);
            boards.All(b => b.BoardSlots.Count == 4).Should().BeTrue();

            foreach (var slot in boards.SelectMany(b => b.BoardSlots))
            {
                slot.GameRoomCard.Should().NotBeNull();
                slot.TargetGameRoomCard.Should().Be(slot.GameRoomCard);
                slot.GameRoomCard!.Location.Should().Be(CardLocation.OnBoard);
                slot.GameRoomCard.CurrentRotation.Should().BeInRange(0, 3);
            }

            var totalAssignedCards = await _context.GameRoomCards.CountAsync(c => c.GameRoomId == room.Id && c.Location == CardLocation.OnBoard);
            totalAssignedCards.Should().Be(8);
        }

        #endregion

        #region DrawBonusCardsAsync & DrawCardsFromDeckAsync Tests

        [Fact]
        public async Task DrawBonusCardsAsync_ShouldReturnRequestedAmountOfCards_AndSetLocationToInHand()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "BNS" };
            _context.GameRooms.Add(room);
            await _context.SaveChangesAsync();

            await _cardManager.CreateDeckForRoomAsync(room);
            await _context.SaveChangesAsync();

            // Act
            var result = await _cardManager.DrawBonusCardsAsync(room.Id, 2);
            await _context.SaveChangesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.All(c => c.Location == CardLocation.InHand).Should().BeTrue();
        }

        [Fact]
        public async Task DrawCardsFromDeckAsync_ShouldReshuffleDiscardIntoDeck_WhenDeckIsTooSmall()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "SHFL" };
            _context.GameRooms.Add(room);
            await _context.SaveChangesAsync();

            for (int i = 1; i <= 10; i++)
            {
                _context.GameRoomCards.Add(new GameRoomCard
                {
                    GameRoomId = room.Id,
                    CardId = i,
                    Location = i == 1 ? CardLocation.InDeck : CardLocation.Discarded
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var drawnCards = await _cardManager.DrawCardsFromDeckAsync(room.Id, 4);

            // Assert
            drawnCards.Should().HaveCount(4);

            var remainingDeckCount = await _context.GameRoomCards
                .CountAsync(c => c.GameRoomId == room.Id && c.Location == CardLocation.InDeck);

            remainingDeckCount.Should().Be(10);
        }

        #endregion

        #region ReleasePlayerCardsToDeckAsync Tests

        [Fact]
        public async Task ReleasePlayerCardsToDeckAsync_ShouldResetBoardSlots_AndReturnAllCardsToDeck()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "RLES" };
            var player = new Player { Name = "Tester", GameRoom = room };
            var board = new Board { Player = player };

            var cardOnBoard = new GameRoomCard { GameRoom = room, Location = CardLocation.OnBoard, CurrentRotation = 2 };
            var bonusCard = new GameRoomCard { GameRoom = room, Location = CardLocation.InHand, CurrentRotation = 1 };

            var slot = new BoardSlot
            {
                Board = board,
                PositionIndex = 0,
                GameRoomCard = cardOnBoard,
                TargetGameRoomCard = cardOnBoard,
                TargetRotation = 2,
                IsCorrect = true
            };

            board.BoardSlots.Add(slot);
            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            _context.Boards.Add(board);
            _context.GameRoomCards.AddRange(cardOnBoard, bonusCard);
            await _context.SaveChangesAsync();

            // Act
            await _cardManager.ReleasePlayerCardsToDeckAsync(room.Id, player.Id);
            await _context.SaveChangesAsync();

            // Assert
            slot.GameRoomCard.Should().BeNull();
            slot.TargetGameRoomCard.Should().BeNull();
            slot.TargetGameRoomCardId.Should().BeNull();
            slot.TargetRotation.Should().Be(0);
            slot.IsCorrect.Should().BeFalse();

            cardOnBoard.Location.Should().Be(CardLocation.InDeck);
            cardOnBoard.CurrentRotation.Should().Be(0);

            bonusCard.Location.Should().Be(CardLocation.InDeck);
            bonusCard.CurrentRotation.Should().Be(0);
        }

        #endregion
    }
}
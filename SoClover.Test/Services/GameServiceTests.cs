using FluentAssertions;
using Moq;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Server.Services;
using SoClover.Tests.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoClover.Tests.Services
{
    public class GameServiceTests : IDisposable
    {
        private readonly SoCloverDBContext _context;
        private readonly Mock<ICardManager> _cardManagerMock;
        private readonly GameService _service;

        public GameServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _cardManagerMock = new Mock<ICardManager>();
            _service = new GameService(_context, _cardManagerMock.Object);
        }

        public void Dispose()
        {
            TestDbContextFactory.Destroy(_context);
        }

        #region StartGameAsync Tests

        [Fact]
        public async Task StartGameAsync_ShouldChangeStatusToWriting_AndCallCardManager_WhenLobbyIsValid()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "ABCD", Status = GameStatus.Lobby };
            var p1 = new Player { ConnectionId = "conn1", Name = "P1", GameRoom = room };
            var p2 = new Player { ConnectionId = "conn2", Name = "P2", GameRoom = room };

            _context.GameRooms.Add(room);
            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.StartGameAsync("conn1");

            // Assert
            result.Status.Should().Be(GameStatus.Writing);

            _cardManagerMock.Verify(cm => cm.AssignCardsToPlayersAsync(
                It.Is<int[]>(list => list.Contains(p1.Id) && list.Contains(p2.Id)),
                room.Id
            ), Times.Once);
        }

        [Fact]
        public async Task StartGameAsync_ShouldThrowException_WhenNotEnoughPlayers()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "ABCD", Status = GameStatus.Lobby };
            var p1 = new Player { ConnectionId = "conn1", Name = "P1", GameRoom = room };

            _context.GameRooms.Add(room);
            _context.Players.Add(p1);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _service.StartGameAsync("conn1");

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Not enough players to start");
        }

        #endregion

        #region SubmitCluesAsync Tests

        [Fact]
        public async Task SubmitCluesAsync_ShouldMoveCardsToHand_SaveClues_AndSetStatusToSolving()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "GAME", Status = GameStatus.Writing };
            var player = new Player { ConnectionId = "player_conn", Name = "Jan", GameRoom = room };
            var board = new Board { Player = player };
            player.Board = board;

            var card = new GameRoomCard { GameRoom = room, Location = CardLocation.OnBoard, CurrentRotation = 1 };
            var slot = new BoardSlot { Board = board, PositionIndex = 0, GameRoomCard = card };
            board.BoardSlots.Add(slot);

            var deckCard1 = new GameRoomCard { GameRoom = room, Location = CardLocation.InDeck };
            var deckCard2 = new GameRoomCard { GameRoom = room, Location = CardLocation.InDeck };

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            _context.Boards.Add(board);
            _context.GameRoomCards.AddRange(card, deckCard1, deckCard2);
            await _context.SaveChangesAsync();

            string[] clues = ["Up", "Right", "Down", "Left"];

            // Act
            var result = await _service.SubmitCluesAsync("player_conn", clues);

            // Assert
            result.Status.Should().Be(GameStatus.Solving);
            result.CheckedPlayer.Should().Be(player);
            player.IsReady.Should().BeTrue();

            board.TopClue.Should().Be("Up");
            board.RightClue.Should().Be("Right");

            slot.GameRoomCard.Should().BeNull();
            slot.TargetGameRoomCard.Should().Be(card);
            slot.TargetRotation.Should().Be(1);
            card.Location.Should().Be(CardLocation.InHand);

            deckCard1.Location.Should().Be(CardLocation.InHand);
            deckCard2.Location.Should().Be(CardLocation.InHand);
        }

        [Fact]
        public async Task SubmitCluesAsync_ShouldThrowException_WhenNotEnoughCluesProvided()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "GAME", Status = GameStatus.Writing };
            var player = new Player { ConnectionId = "player_conn", Name = "Jan", GameRoom = room };
            player.Board = new Board { Player = player };

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            string[] incompleteClues = ["Up", "Right", "Down"];

            // Act
            Func<Task> act = async () => await _service.SubmitCluesAsync("player_conn", incompleteClues);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Not enough clues provided");
        }

        #endregion

        #region CreateBoardDTOsForPlayersInRoom Tests

        [Fact]
        public async Task CreateBoardDTOsForPlayersInRoom_ShouldReturnCorrectDTOs_InLobbyOrWritingState()
        {
            // Arrange
            var room = new GameRoom { Status = GameStatus.Writing };
            var p1 = new Player { Name = "P1", GameRoom = room, Board = new Board { TopClue = "P1_Top" } };
            var p2 = new Player { Name = "P2", GameRoom = room, Board = new Board { TopClue = "P2_Top" } };

            _context.GameRooms.Add(room);
            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CreateBoardDTOsForPlayersInRoom(room.Id);

            // Assert
            result.Should().HaveCount(2);
            result[p1].TopClue.Should().Be("P1_Top");
            result[p2].TopClue.Should().Be("P2_Top");
        }

        [Fact]
        public async Task CreateBoardDTOsForPlayersInRoom_ShouldReturnCheckedPlayerBoard_ForEveryone_InSolvingState()
        {
            // Arrange
            var room = new GameRoom { Status = GameStatus.Solving };
            var p1 = new Player { Name = "P1", GameRoom = room, Board = new Board { TopClue = "P1_Top" } };
            var p2 = new Player { Name = "P2", GameRoom = room, Board = new Board { TopClue = "P2_Top" } };

            room.CheckedPlayer = p1;

            _context.GameRooms.Add(room);
            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CreateBoardDTOsForPlayersInRoom(room.Id);

            // Assert
            result.Should().HaveCount(2);
            result[p1].TopClue.Should().Be("P1_Top");
            result[p2].TopClue.Should().Be("P1_Top");
        }

        #endregion

        #region RotateCard Tests

        [Fact]
        public async Task RotateCard_ShouldIncrementRotation_AndWrapAroundAtFour()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "ROTA" };
            var card = new GameRoomCard { GameRoom = room, CurrentRotation = 3 };

            _context.GameRooms.Add(room);
            _context.GameRoomCards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            var resultRoom = await _service.RotateCard(card.Id);

            // Assert
            card.CurrentRotation.Should().Be(0);
            resultRoom.Id.Should().Be(room.Id);
        }

        #endregion
    }
}
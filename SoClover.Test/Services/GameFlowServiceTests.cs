using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Server.Services;
using SoClover.Tests.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SoClover.Tests.Services
{
    public class GameFlowServiceTests : IDisposable
    {
        private readonly SoCloverDBContext _context;
        private readonly Mock<ICardManager> _cardManagerMock;
        private readonly GameFlowService _service;

        public GameFlowServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _cardManagerMock = new Mock<ICardManager>();

            _service = new GameFlowService(_context, _cardManagerMock.Object);
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
            var p1Guid = Guid.NewGuid();
            var p2Guid = Guid.NewGuid();

            var p1 = new Player { PlayerGuid = p1Guid, Name = "P1", GameRoom = room };
            var p2 = new Player { PlayerGuid = p2Guid, Name = "P2", GameRoom = room };

            _context.GameRooms.Add(room);
            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.StartGameAsync(p1Guid);

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
            var p1Guid = Guid.NewGuid();
            var p1 = new Player { PlayerGuid = p1Guid, Name = "P1", GameRoom = room };

            _context.GameRooms.Add(room);
            _context.Players.Add(p1);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _service.StartGameAsync(p1Guid);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Not enough players to start");
        }

        #endregion

        #region SubmitCluesAsync Tests

        [Fact]
        public async Task SubmitCluesAsync_ShouldMoveCardsToHand_SaveClues_AndCallDrawBonusCards()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "GAME", Status = GameStatus.Writing };
            var pGuid = Guid.NewGuid();
            var player = new Player { PlayerGuid = pGuid, Name = "Jan", GameRoom = room };
            var board = new Board { Player = player };
            player.Board = board;

            var card = new GameRoomCard { GameRoom = room, Location = CardLocation.OnBoard, CurrentRotation = 1 };
            var slot = new BoardSlot { Board = board, PositionIndex = 0, GameRoomCard = card };
            board.BoardSlots.Add(slot);

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            _context.Boards.Add(board);
            _context.GameRoomCards.Add(card);
            await _context.SaveChangesAsync();

            string[] clues = ["Up", "Right", "Down", "Left"];

            _cardManagerMock.Setup(cm => cm.DrawBonusCardsAsync(room.Id, 2))
                .ReturnsAsync(new List<GameRoomCard>());

            // Act
            var result = await _service.SubmitCluesAsync(pGuid, clues);

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

            _cardManagerMock.Verify(cm => cm.DrawBonusCardsAsync(room.Id, 2), Times.Once);
        }

        [Fact]
        public async Task SubmitCluesAsync_ShouldThrowException_WhenNotEnoughCluesProvided()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "GAME", Status = GameStatus.Writing };
            var pGuid = Guid.NewGuid();
            var player = new Player { PlayerGuid = pGuid, Name = "Jan", GameRoom = room };
            player.Board = new Board { Player = player };

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            string[] incompleteClues = ["Up", "Right", "Down"];

            // Act
            Func<Task> act = async () => await _service.SubmitCluesAsync(pGuid, incompleteClues);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Not enough clues provided");
        }

        #endregion

        #region ReturnToWritingAsync Tests

        [Fact]
        public async Task ReturnToWritingAsync_ShouldResetState_CallReleaseAndReassignCards()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "BACK", Status = GameStatus.Solving };
            var pGuid = Guid.NewGuid();
            var player = new Player { PlayerGuid = pGuid, Name = "Jan", GameRoom = room, IsReady = true };
            var board = new Board { Player = player, TopClue = "OldClue", IsActive = true };
            player.Board = board;
            room.CheckedPlayer = player;

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ReturnToWritingAsync(pGuid);

            // Assert
            result.Status.Should().Be(GameStatus.Writing);
            result.CheckedPlayer.Should().BeNull();
            player.IsReady.Should().BeFalse();

            board.TopClue.Should().BeEmpty();
            board.IsActive.Should().BeFalse();

            _cardManagerMock.Verify(cm => cm.ReleasePlayerCardsToDeckAsync(room.Id, player.Id), Times.Once);

            _cardManagerMock.Verify(cm => cm.AssignCardsToPlayersAsync(
                It.Is<int[]>(list => list.Length == 1 && list.Contains(player.Id)),
                room.Id
            ), Times.Once);
        }

        [Fact]
        public async Task ReturnToWritingAsync_ShouldThrowException_WhenGameIsNotInSolvingState()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "BACK", Status = GameStatus.Lobby };
            var pGuid = Guid.NewGuid();
            var player = new Player { PlayerGuid = pGuid, Name = "Jan", GameRoom = room };

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _service.ReturnToWritingAsync(pGuid);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Can only rollback from Solving state");
        }

        #endregion
    }
}
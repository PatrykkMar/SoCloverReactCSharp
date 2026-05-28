using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoClover.Server.Context;
using SoClover.Server.Helpers;
using SoClover.Server.Models;
using SoClover.Server.Services;
using SoClover.Tests.Context;


namespace SoClover.Tests.Services
{
    public class RoomServiceTests : IDisposable
    {
        private readonly SoCloverDBContext _context;
        private readonly Mock<ICardManager> _cardManagerMock;
        private readonly RoomService _service;

        public RoomServiceTests()
        {
            _context = TestDbContextFactory.Create();

            _cardManagerMock = new Mock<ICardManager>();

            _service = new RoomService(_context, _cardManagerMock.Object);
        }

        public void Dispose()
        {
            TestDbContextFactory.Destroy(_context);
        }

        #region CreateRoomAsync Tests

        [Fact]
        public async Task CreateRoomAsync_ShouldCreateRoom_AndCallCardManager()
        {
            // Act
            var result = await _service.CreateRoomAsync();

            // Assert
            result.Should().NotBeNull();
            result.RoomCode.Should().HaveLength(4);
            result.Status.Should().Be(GameStatus.Lobby);

            var roomInDb = await _context.GameRooms.FirstOrDefaultAsync(r => r.Id == result.Id);
            roomInDb.Should().NotBeNull();

            _cardManagerMock.Verify(
                cm => cm.CreateDeckForRoomAsync(It.IsAny<GameRoom>()),
                Times.Once
            );
        }

        #endregion

        #region JoinRoomAsync Tests

        [Fact]
        public async Task JoinRoomAsync_ShouldAddPlayer_WhenRoomExistsAndHasSpace()
        {
            // Arrange
            var room = await _service.CreateRoomAsync();
            var playerGuid = Guid.NewGuid();

            // Act
            var result = await _service.JoinRoomAsync(room.RoomCode, "Player1", playerGuid);

            // Assert
            result.Should().NotBeNull();
            result.Players.Should().ContainSingle();
            result.Players.First().Should().Be("Player1");
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldReturnExistingData_WhenPlayerIsAlreadyInRoom()
        {
            // Arrange
            var room = await _service.CreateRoomAsync();
            var playerGuid = Guid.NewGuid();

            await _service.JoinRoomAsync(room.RoomCode, "Player1", playerGuid);

            // Act
            var result = await _service.JoinRoomAsync(room.RoomCode, "Player1", playerGuid);

            // Assert
            result.Players.Should().ContainSingle();
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldThrowException_WhenRoomDoesNotExist()
        {
            // Act
            Func<Task> act = async () => await _service.JoinRoomAsync("NONX", "Player", Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Room does not exist");
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldThrowException_WhenGameHasAlreadyStarted()
        {
            // Arrange
            var room = await _service.CreateRoomAsync();
            room.Status = GameStatus.Writing;
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _service.JoinRoomAsync(room.RoomCode, "Player", Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Game just started");
        }

        [Fact]
        public async Task JoinRoomAsync_ShouldThrowException_WhenRoomIsFull()
        {
            // Arrange
            var room = await _service.CreateRoomAsync();

            for (int i = 0; i < 6; i++)
            {
                room.Players.Add(new Player { Name = $"P{i}", PlayerGuid = Guid.NewGuid() });
            }
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _service.JoinRoomAsync(room.RoomCode, "ExtraPlayer", Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Room is full");
        }

        #endregion

        #region LeaveRoomAsync Tests

        [Fact]
        public async Task LeaveRoomAsync_ShouldRemovePlayer_AndKeepRoom_WhenOtherPlayersAreLeft()
        {
            // Arrange
            var room = await _service.CreateRoomAsync();

            var p1Guid = Guid.NewGuid();
            var p2Guid = Guid.NewGuid();
            await _service.JoinRoomAsync(room.RoomCode, "P1", p1Guid);
            await _service.JoinRoomAsync(room.RoomCode, "P2", p2Guid);

            // Act
            var result = await _service.LeaveRoomAsync(p1Guid);

            // Assert
            result.Should().NotBeNull();
            result!.Players.Should().ContainSingle();
            result.Players.First().Should().Be("P2");

            var playerInDb = await _context.Players.AnyAsync(p => p.PlayerGuid == p1Guid);
            playerInDb.Should().BeFalse();
        }

        [Fact]
        public async Task LeaveRoomAsync_ShouldDeleteRoom_WhenLastPlayerLeaves()
        {
            // Arrange
            var room = await _service.CreateRoomAsync();
            var pGuid = Guid.NewGuid();
            await _service.JoinRoomAsync(room.RoomCode, "LastPlayer", pGuid);

            // Act
            var result = await _service.LeaveRoomAsync(pGuid);

            // Assert
            result.Should().BeNull();

            var roomInDb = await _context.GameRooms.AnyAsync(r => r.Id == room.Id);
            roomInDb.Should().BeFalse();
        }

        [Fact]
        public async Task LeaveRoomAsync_ShouldReturnNull_WhenPlayerGuidNotFound()
        {
            // Act
            var result = await _service.LeaveRoomAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        #endregion
    }
}
using FluentAssertions;
using SoClover.Server.Context;
using SoClover.Server.Models;
using SoClover.Server.Services;
using SoClover.Tests.Context;

namespace SoClover.Tests.Services
{
    public class BoardServiceTests : IDisposable
    {
        private readonly SoCloverDBContext _context;
        private readonly BoardService _service;

        public BoardServiceTests()
        {
            _context = TestDbContextFactory.Create();
            _service = new BoardService(_context);
        }

        public void Dispose()
        {
            TestDbContextFactory.Destroy(_context);
        }

        #region CreateBoardDTOsForPlayersInRoomAsync Tests

        [Fact]
        public async Task CreateBoardDTOsForPlayersInRoomAsync_ShouldReturnIndividualBoards_WhenStatusIsNotSolving()
        {
            // Arrange
            var room = new GameRoom { Status = GameStatus.Writing };
            var p1 = new Player { Name = "P1", PlayerGuid = Guid.NewGuid(), GameRoom = room };
            var p2 = new Player { Name = "P2", PlayerGuid = Guid.NewGuid(), GameRoom = room };

            p1.Board = new Board { Player = p1, TopClue = "Clue1" };
            p2.Board = new Board { Player = p2, TopClue = "Clue2" };

            _context.GameRooms.Add(room);
            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CreateBoardDTOsForPlayersInRoomAsync(room.Id);

            // Assert
            result.Should().HaveCount(2);

            result[p1].Should().NotBeNull();
            result[p2].Should().NotBeNull();
        }

        [Fact]
        public async Task CreateBoardDTOsForPlayersInRoomAsync_ShouldReturnCheckedPlayerBoardForEveryone_WhenStatusIsSolving()
        {
            // Arrange
            var room = new GameRoom { Status = GameStatus.Solving };
            var p1 = new Player { Name = "P1", PlayerGuid = Guid.NewGuid(), GameRoom = room };
            var p2 = new Player { Name = "P2", PlayerGuid = Guid.NewGuid(), GameRoom = room };

            var checkedBoard = new Board { Player = p1, TopClue = "SecretClue" };
            p1.Board = checkedBoard;
            p2.Board = new Board { Player = p2, TopClue = "OtherClue" };
            room.CheckedPlayer = p1;

            _context.GameRooms.Add(room);
            _context.Players.AddRange(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CreateBoardDTOsForPlayersInRoomAsync(room.Id);

            // Assert
            result.Should().HaveCount(2);

            result[p1].Should().NotBeNull();
            result[p2].Should().NotBeNull();
        }

        #endregion

        #region RotateCardAsync Tests

        [Theory]
        [InlineData(0, 1)]
        [InlineData(3, 0)]
        public async Task RotateCardAsync_ShouldIncrementRotationAndWrapAroundAtFour(int initialRotation, int expectedRotation)
        {
            // Arrange
            var room = new GameRoom { RoomCode = "ROTA" };
            var card = new GameRoomCard { GameRoom = room, CurrentRotation = initialRotation };

            _context.GameRooms.Add(room);
            _context.GameRoomCards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            var resultRoom = await _service.RotateCardAsync(card.Id);

            // Assert
            card.CurrentRotation.Should().Be(expectedRotation);
            resultRoom.Id.Should().Be(room.Id);
        }

        #endregion

        #region MoveCardToSlotAsync Tests

        [Fact]
        public async Task MoveCardToSlotAsync_ShouldPlaceCardOnSlot_AndChangeLocationToOnBoard()
        {
            // Arrange
            var room = new GameRoom { Id = 2, RoomCode = "MOVE" };
            var pGuid = Guid.NewGuid();

            var player = new Player { Id = 20, PlayerGuid = pGuid, Name = "Jan", GameRoomId = 2, GameRoom = room };
            var board = new Board { Id = 200, PlayerId = 20, Player = player, IsActive = true };
            player.Board = board;

            var slot = new BoardSlot { Id = 2001, BoardId = 200, Board = board, PositionIndex = 2, IsCorrect = false };
            board.BoardSlots.Add(slot);
            room.Players.Add(player);

            var card = new GameRoomCard { Id = 201, GameRoomId = 2, GameRoom = room, Location = CardLocation.InHand };

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            _context.Boards.Add(board);
            _context.GameRoomCards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            var resultRoom = await _service.MoveCardToSlotAsync(pGuid, card.Id, 2);

            // Assert
            resultRoom.Should().NotBeNull();
            slot.GameRoomCard.Should().NotBeNull();
            slot.GameRoomCard!.Id.Should().Be(card.Id);
            card.Location.Should().Be(CardLocation.OnBoard);
        }

        [Fact]
        public async Task MoveCardToSlotAsync_ShouldReturnOldCardToHand_WhenSlotIsAlreadyOccupied()
        {
            // Arrange
            var room = new GameRoom { Id = 3, RoomCode = "SWAP" };
            var pGuid = Guid.NewGuid();

            var player = new Player { Id = 30, PlayerGuid = pGuid, GameRoomId = 3, GameRoom = room };
            var board = new Board { Id = 300, PlayerId = 30, Player = player, IsActive = true };
            player.Board = board;

            var oldCard = new GameRoomCard { Id = 301, GameRoomId = 3, GameRoom = room, Location = CardLocation.OnBoard };
            var movingCard = new GameRoomCard { Id = 302, GameRoomId = 3, GameRoom = room, Location = CardLocation.InHand };

            var slot = new BoardSlot { Id = 3001, BoardId = 300, Board = board, PositionIndex = 0, GameRoomCard = oldCard };
            board.BoardSlots.Add(slot);
            room.Players.Add(player);

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            _context.Boards.Add(board);
            _context.GameRoomCards.AddRange(oldCard, movingCard);
            await _context.SaveChangesAsync();

            // Act
            await _service.MoveCardToSlotAsync(pGuid, movingCard.Id, 0);

            // Assert
            slot.GameRoomCard.Should().NotBeNull();
            slot.GameRoomCard!.Id.Should().Be(movingCard.Id);
            oldCard.Location.Should().Be(CardLocation.InHand);
        }

        [Fact]
        public async Task MoveCardToSlotAsync_ShouldThrowException_WhenSlotIsAlreadyMarkedAsCorrect()
        {
            // Arrange
            var room = new GameRoom { Id = 4, RoomCode = "LOCK" };
            var pGuid = Guid.NewGuid();

            var player = new Player { Id = 40, PlayerGuid = pGuid, Name = "Jan", GameRoomId = 4, GameRoom = room };
            var board = new Board { Id = 400, PlayerId = 40, Player = player, IsActive = true };
            player.Board = board;

            var slot = new BoardSlot { Id = 4001, BoardId = 400, Board = board, PositionIndex = 1, IsCorrect = true };
            board.BoardSlots.Add(slot);
            room.Players.Add(player);

            var card = new GameRoomCard { Id = 401, GameRoomId = 4, GameRoom = room, Location = CardLocation.InHand };

            _context.GameRooms.Add(room);
            _context.Players.Add(player);
            _context.Boards.Add(board);
            _context.GameRoomCards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _service.MoveCardToSlotAsync(pGuid, card.Id, 1);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Target slot is correct, therefore it can't be moved");
        }

        #endregion

        #region CheckAsync Tests

        [Fact]
        public async Task CheckAsync_ShouldMarkSlotsAsCorrect_WhenCardsAndRotationsMatchTargets()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "CHCK" };
            var pGuid = Guid.NewGuid();
            var player = new Player { PlayerGuid = pGuid, GameRoom = room };
            room.Players.Add(player);

            var board = new Board { Player = player, IsActive = true };
            var card = new GameRoomCard { Id = 99, GameRoom = room, CurrentRotation = 2 };

            var slot = new BoardSlot
            {
                Board = board,
                GameRoomCard = card,
                TargetGameRoomCardId = 99,
                TargetRotation = 2 
            };

            board.BoardSlots.Add(slot);
            _context.GameRooms.Add(room);
            _context.Boards.Add(board);
            _context.GameRoomCards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            var resultRoom = await _service.CheckAsync(pGuid);

            // Assert
            resultRoom.Id.Should().Be(room.Id);
            slot.IsCorrect.Should().BeTrue();
        }

        [Fact]
        public async Task CheckAsync_ShouldMarkSlotsAsIncorrect_WhenRotationDoesNotMatch()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "FAIL" };
            var pGuid = Guid.NewGuid();
            var player = new Player { PlayerGuid = pGuid, GameRoom = room };
            room.Players.Add(player);

            var board = new Board { Player = player, IsActive = true };
            var card = new GameRoomCard { Id = 55, GameRoom = room, CurrentRotation = 0 };

            var slot = new BoardSlot
            {
                Board = board,
                GameRoomCard = card,
                TargetGameRoomCardId = 55,
                TargetRotation = 2
            };

            board.BoardSlots.Add(slot);
            _context.GameRooms.Add(room);
            _context.Boards.Add(board);
            _context.GameRoomCards.Add(card);
            await _context.SaveChangesAsync();

            // Act
            await _service.CheckAsync(pGuid);

            // Assert
            slot.IsCorrect.Should().BeFalse();
        }

        [Fact]
        public async Task CheckAsync_ShouldThrowException_WhenAnySlotIsEmpty()
        {
            // Arrange
            var room = new GameRoom { RoomCode = "EMPT" };
            var pGuid = Guid.NewGuid();
            var player = new Player { PlayerGuid = pGuid, GameRoom = room };
            room.Players.Add(player);

            var board = new Board { Player = player, IsActive = true };
            var slot = new BoardSlot { Board = board, GameRoomCard = null };

            board.BoardSlots.Add(slot);
            _context.GameRooms.Add(room);
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _service.CheckAsync(pGuid);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Not all slots are filled");
        }

        #endregion
    }
}
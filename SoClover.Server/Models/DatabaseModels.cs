using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoClover.Server.Models
{
    public enum GameStatus { Lobby, Writing, Solving, Finished }

    public enum CardLocation
    {
        InDeck, InHand, OnBoard, Discarded
    }

    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
    }

    public class GameRoom : BaseEntity
    {

        [Required, StringLength(4), Column(TypeName = "nchar(4)")]
        public string RoomCode { get; set; } = string.Empty;

        public GameStatus Status { get; set; }

        public int? ActivePlayerId { get; set; }



        public Player? ActivePlayer { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<GameRoomCard> GameRoomCards { get; set; } = new List<GameRoomCard>();
    }

    public class Player : BaseEntity
    {
        [Required]
        public Guid PlayerGuid { get; set; }

        [Required, StringLength(30)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ConnectionId { get; set; }

        public int GameRoomId { get; set; }
        public bool IsReady { get; set; }
        public int Score { get; set; }


        public GameRoom GameRoom { get; set; } = null!;
        public Board? Board { get; set; }
    }

    public class Card
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string WordTop { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string WordRight { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string WordBottom { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string WordLeft { get; set; } = string.Empty;

        public ICollection<GameRoomCard> GameRoomCards { get; set; } = new List<GameRoomCard>();
    }

    public class Board : BaseEntity
    {
        public int PlayerId { get; set; }

        [StringLength(50)]
        public string? TopClue { get; set; }

        [StringLength(50)]
        public string? RightClue { get; set; }

        [StringLength(50)]
        public string? BottomClue { get; set; }

        [StringLength(50)]
        public string? LeftClue { get; set; }
        public bool IsActive { get; set; }

        public Player Player { get; set; } = null!;
        public ICollection<BoardSlot> BoardSlots { get; set; } = new List<BoardSlot>();
    }

    public class BoardSlot : BaseEntity
    {
        public int BoardId { get; set; }
        public int? GameRoomCardId { get; set; }
        public bool IsCorrect { get; set; }
        public int PositionIndex { get; set; } // 0-3
        public int? TargetGameRoomCardId { get; set; }
        public int? TargetRotation { get; set; }
        public GameRoomCard? TargetGameRoomCard { get; set; }
        public Board Board { get; set; } = null!;
        public GameRoomCard? GameRoomCard { get; set; }
    }

    public class GameRoomCard : BaseEntity //deck
    {
        public int GameRoomId { get; set; }
        public int CardId { get; set; }
        public int CurrentRotation { get; set; } // 0, 90, 180, 270

        public CardLocation Location { get; set; }

        public GameRoom GameRoom { get; set; } = null!;
        public Card Card { get; set; } = null!;
        public BoardSlot? BoardSlot { get; set; }
        public ICollection<BoardSlot> TargetBoardSlots { get; set; } = new List<BoardSlot>();
    }
}
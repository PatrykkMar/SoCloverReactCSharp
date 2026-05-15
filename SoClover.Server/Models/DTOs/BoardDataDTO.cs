namespace SoClover.Server.Models.DTOs
{
    public class BoardDataDTO
    {
        public BoardDataDTO(Board board) 
        {
            Cards = [.. board.BoardSlots.Select(bs => new CardDTO
            {
                WordTop = bs.GameRoomCard?.Card.WordTop ?? string.Empty,
                WordRight = bs.GameRoomCard?.Card.WordRight ?? string.Empty,
                WordBottom = bs.GameRoomCard?.Card.WordBottom ?? string.Empty,
                WordLeft = bs.GameRoomCard?.Card.WordLeft ?? string.Empty,
                CurrentRotation = bs.GameRoomCard?.CurrentRotation ?? 0,
                PositionIndex = bs.GameRoomCard?.PositionIndex ?? 0
            })];
            IsActive = board.IsActive;

            TopClue = board.TopClue;
            RightClue = board.RightClue;
            BottomClue = board.BottomClue;
            LeftClue = board.LeftClue;
        }

        public CardDTO[] Cards { get; set; } = [];
        public string? TopClue { get; set; } 
        public string? RightClue { get; set; } 
        public string? BottomClue { get; set; } 
        public string? LeftClue { get; set; }
        public bool IsActive { get; set; } = false;
    }

    public class CardDTO
    {
        public string WordTop { get; set; } = string.Empty;
        public string WordRight { get; set; } = string.Empty;
        public string WordBottom { get; set; } = string.Empty;
        public string WordLeft { get; set; } = string.Empty;
        public int CurrentRotation { get; set; } = 0;
        public int PositionIndex { get; set; } = 0;
    }
}

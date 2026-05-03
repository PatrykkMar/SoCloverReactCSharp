namespace SoClover.Server.Models.DTOs
{
    public class BoardDataDTO
    {
        public BoardDataDTO(Board board) 
        {
            BoardSlots = [.. board.BoardSlots.Select(bs => new BoardSlotDTO
            {
                Card = bs.GameRoomCard != null ? new CardDTO
                {
                    WordTop = bs.GameRoomCard.Card.WordTop,
                    WordRight = bs.GameRoomCard.Card.WordRight,
                    WordBottom = bs.GameRoomCard.Card.WordBottom,
                    WordLeft = bs.GameRoomCard.Card.WordLeft
                } : null,
                CurrentRotation = bs.CurrentRotation,
                PositionIndex = bs.PositionIndex
            }).ToArray()];
        }

        public BoardSlotDTO[] BoardSlots { get; set; } = [];
        public string TopClue { get; set; } = string.Empty;
        public string RightClue { get; set; } = string.Empty;
        public string BottomClue { get; set; } = string.Empty;
        public string LeftClue { get; set; }= string.Empty;
    }

    public class BoardSlotDTO
    {
        public CardDTO? Card { get; set; } = null;
        public int CurrentRotation { get; set; } = 0;
        public int PositionIndex { get; set; } = 0;
    }

    public class CardDTO
    {
        public string WordTop { get; set; } = string.Empty;
        public string WordRight { get; set; } = string.Empty;
        public string WordBottom { get; set; } = string.Empty;
        public string WordLeft { get; set; } = string.Empty;
    }
}

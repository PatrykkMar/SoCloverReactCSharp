namespace SoClover.Server.Models.DTOs
{
    public class BoardDTO
    {
        public BoardSlotDTO[] BoardSlots { get; set; } = [];
    }

    public class BoardSlotDTO
    {
        public CardDTO Card { get; set; } = new CardDTO();
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

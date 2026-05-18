namespace SoClover.Server.Models.DTOs
{
    public class BoardDataDTO
    {
        public BoardDataDTO(Board board, bool isActive = true, bool includingHand = false) 
        {
            var cardsOnBoard = board.BoardSlots.Select(bs => new CardDTO
            {
                GameRoomCardId = bs.GameRoomCardId ?? 0,
                WordTop = bs.GameRoomCard?.Card.WordTop ?? string.Empty,
                WordRight = bs.GameRoomCard?.Card.WordRight ?? string.Empty,
                WordBottom = bs.GameRoomCard?.Card.WordBottom ?? string.Empty,
                WordLeft = bs.GameRoomCard?.Card.WordLeft ?? string.Empty,
                CurrentRotation = bs.GameRoomCard?.CurrentRotation ?? 0,
                PositionIndex = bs.PositionIndex,
                Location = CardLocation.OnBoard.ToString()
            });

            var cardsInHand = includingHand ? board.Player.GameRoom.GameRoomCards.Where(grc => grc.Location == CardLocation.InHand).Select(grc => new CardDTO
            {
                GameRoomCardId = grc.Id,
                WordTop = grc.Card.WordTop,
                WordRight = grc.Card.WordRight,
                WordBottom = grc.Card.WordBottom,
                WordLeft = grc.Card.WordLeft,
                CurrentRotation = grc.CurrentRotation,
                Location = CardLocation.InHand.ToString()
            }) : [];  

            Cards = [.. cardsOnBoard, .. cardsInHand];
            TopClue = board.TopClue;
            RightClue = board.RightClue;
            BottomClue = board.BottomClue;
            LeftClue = board.LeftClue;
            InputsActive = isActive;
        }

        public CardDTO[] Cards { get; set; } = [];
        public CardDTO[] Hand { get; set; } = [];
        public string? TopClue { get; set; } 
        public string? RightClue { get; set; } 
        public string? BottomClue { get; set; } 
        public string? LeftClue { get; set; }
        public bool InputsActive { get; set; } = false;
        public bool CardsActive { get; set; } = false;
        public string Location { get; set; } = string.Empty;
    }

    public class CardDTO
    {
        public int GameRoomCardId { get; set; }
        public string WordTop { get; set; } = string.Empty;
        public string WordRight { get; set; } = string.Empty;
        public string WordBottom { get; set; } = string.Empty;
        public string WordLeft { get; set; } = string.Empty;
        public int CurrentRotation { get; set; } = 0;
        public int? PositionIndex { get; set; } = null;
        public string Location { get; set; } = string.Empty;
    }
}

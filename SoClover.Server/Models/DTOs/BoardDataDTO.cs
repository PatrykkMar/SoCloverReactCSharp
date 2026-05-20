namespace SoClover.Server.Models.DTOs
{
    public class BoardDataDTO
    {
        public BoardDataDTO(Board board, bool isActive = true, bool includingHand = false) 
        {
            var room = board.Player.GameRoom;
            if(room.Status == GameStatus.Writing)
            {
                Cards = board.BoardSlots.Select(bs => new CardDTO
                {
                    GameRoomCardId = bs.GameRoomCardId ?? 0,
                    WordTop = bs.GameRoomCard?.Card.WordTop ?? string.Empty,
                    WordRight = bs.GameRoomCard?.Card.WordRight ?? string.Empty,
                    WordBottom = bs.GameRoomCard?.Card.WordBottom ?? string.Empty,
                    WordLeft = bs.GameRoomCard?.Card.WordLeft ?? string.Empty,
                    CurrentRotation = bs.GameRoomCard?.CurrentRotation ?? 0,
                    PositionIndex = bs.PositionIndex,
                    Location = CardLocation.OnBoard.ToString()
                }).ToArray();
            }
            else if(room.Status == GameStatus.Solving)
            {
                Cards = room.GameRoomCards.Where(x=> (new[] { CardLocation.OnBoard, CardLocation.InHand }).Contains(x.Location)).Select(grc => new CardDTO
                {
                    GameRoomCardId = grc.Id,
                    WordTop = grc.Card.WordTop,
                    WordRight = grc.Card.WordRight,
                    WordBottom = grc.Card.WordBottom,
                    WordLeft = grc.Card.WordLeft,
                    CurrentRotation = grc.CurrentRotation,
                    PositionIndex = board.BoardSlots.FirstOrDefault(bs => bs.GameRoomCardId == grc.Id)?.PositionIndex,
                    Location = grc.Location.ToString()
                }).OrderBy(grc => grc.GameRoomCardId).ToArray();
            }
            else
            {
                throw new Exception("Invalid game status for board data");
            }
            TopClue = board.TopClue;
            RightClue = board.RightClue;
            BottomClue = board.BottomClue;
            LeftClue = board.LeftClue;
            InputsActive = isActive;
        }

        public CardDTO[] Cards { get; set; } = [];
        public string? TopClue { get; set; } 
        public string? RightClue { get; set; } 
        public string? BottomClue { get; set; } 
        public string? LeftClue { get; set; }
        public bool InputsActive { get; set; } = false;
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

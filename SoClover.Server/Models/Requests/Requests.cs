namespace SoClover.Server.Models.Requests
{
    public class CreateRoomRequest
    {
        public string PlayerName { get; set; } = string.Empty;
    }

    public class JoinRoomRequest
    {
        public string RoomCode { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
    }


    public class SubmitCluesRequest
    {
        public string[] Words { get; set; } = [];
    }

    public class RotateCardRequest
    {
        public int GameRoomCardId { get; set; }
    }

    public class MoveCardRequest
    {
        public int GameRoomCardId { get; set; }
        public int PositionIndex { get; set; }
    }
}

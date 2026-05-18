namespace SoClover.Server.Models.Requests
{
    public class CreateRoomRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public Guid PlayerGuid { get; set; }
    }

    public class JoinRoomRequest
    {
        public string RoomCode { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public Guid PlayerGuid { get; set; }
    }


    public class SubmitCluesRequest
    {
        public string[] Words { get; set; } = [];
    }

    public class RotateCardRequest
    {
        public int GameRoomCardId { get; set; }
    }
}

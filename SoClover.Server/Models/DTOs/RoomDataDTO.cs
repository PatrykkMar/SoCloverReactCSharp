namespace SoClover.Server.Models.DTOs
{
    public class RoomDataDTO
    {
        public string RoomCode { get; set; }
        public string[] Players { get; set; }
        public string Status { get; set; }
        public int NumberOfAttempts {  get; set; }
        public RoomDataDTO(GameRoom room)
        {
            RoomCode = room.RoomCode;
            Players = room.Players.Select(p => p.Name).ToArray();
            Status = room.Status.ToString();
            NumberOfAttempts = room.NumberOfAttempts;
        }
    }
}

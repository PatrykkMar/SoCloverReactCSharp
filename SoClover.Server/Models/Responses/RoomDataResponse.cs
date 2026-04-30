namespace SoClover.Server.Models.Responses
{
    public class RoomDataDto
    {
        public string RoomCode { get; set; }
        public string[] Players { get; set; }
        public RoomDataDto(GameRoom room)
        {
            if (room == null || room.Players.Count == 0)
                throw new Exception("No room or empty room");
            RoomCode = room.RoomCode;
            Players = [.. room.Players.Select(p => p.Name).ToArray()];
        }
    }
}

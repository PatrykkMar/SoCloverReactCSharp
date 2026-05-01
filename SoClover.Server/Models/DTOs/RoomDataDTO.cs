namespace SoClover.Server.Models.DTOs
{
    public class RoomDataDTO
    {
        public string RoomCode { get; set; }
        public string[] Players { get; set; }
        public RoomDataDTO(GameRoom room)
        {
            if (room == null || room.Players.Count == 0)
                throw new Exception("No room or empty room");
            RoomCode = room.RoomCode;
            Players = [.. room.Players.Select(p => p.Name).ToArray()];
        }

        public BoardDTO? Board { get; set; } = null;
    }
}

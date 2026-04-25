namespace SoClover.Server.Services
{
    public interface IRoomService
    {
        public Task CreateRoomAsync();
        public Task JoinRoomAsync();
        public Task LeaveRoomAsync();
    }

    public class RoomService : IRoomService
    {
        public Task CreateRoomAsync()
        {
            throw new NotImplementedException();
        }

        public Task JoinRoomAsync()
        {
            throw new NotImplementedException();
        }

        public Task LeaveRoomAsync()
        {
            throw new NotImplementedException();
        }
    }
}

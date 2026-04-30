using Microsoft.AspNetCore.SignalR;
using SoClover.Server.Services;

namespace SoClover.Server.Hubs
{
    public class SoCloverHub(IRoomService service) : Hub
    {
        private readonly IRoomService _roomService = service;

        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
        }

        public async Task CreateRoom()
        {
            var room = await _roomService.CreateRoomAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode);
            await Clients.Caller.SendAsync("RoomCreated", room.RoomCode);
        }

        public async Task JoinRoom(string roomCode, string playerName)
        {
            try
            {
                var player = await _roomService.JoinRoomAsync(roomCode, playerName, Context.ConnectionId);

                await Groups.AddToGroupAsync(Context.ConnectionId, roomCode.ToUpper());

                await Clients.Group(roomCode.ToUpper()).SendAsync("PlayerJoined", player);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomId = await _roomService.LeaveRoomAsync(Context.ConnectionId);

            if (roomId.HasValue)
            {
                await Clients.All.SendAsync("PlayerLeft", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

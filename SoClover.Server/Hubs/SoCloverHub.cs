using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using SoClover.Server.Models;
using SoClover.Server.Models.Requests;
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

        public async Task CreateRoom(CreateRoomRequest request)
        {
            var room = await _roomService.CreateRoomAsync();
            var roomData = await _roomService.JoinRoomAsync(room.RoomCode, request.PlayerName, Context.ConnectionId, request.PlayerGuid);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
        }

        public async Task JoinRoom(JoinRoomRequest request)
        {
            try
            {
                var roomData = await _roomService.JoinRoomAsync(
                    request.RoomCode,
                    request.PlayerName,
                    Context.ConnectionId,
                    request.PlayerGuid
                );

                await Groups.AddToGroupAsync(Context.ConnectionId, roomData.RoomCode.ToUpper());

                await Clients.Group(roomData.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomData = await _roomService.LeaveRoomAsync(Context.ConnectionId);

            if (roomData != null)
            {
                await Clients.All.SendAsync("RoomUpdated", roomData);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using SoClover.Server.Models;
using SoClover.Server.Models.Requests;
using SoClover.Server.Services;

namespace SoClover.Server.Hubs
{
    public class SoCloverHub(IRoomService roomService, IGameService gameService) : Hub
    {
        private readonly IRoomService _roomService = roomService;
        private readonly IGameService _gameService = gameService;

        public override async Task OnConnectedAsync()
        {

            await base.OnConnectedAsync();
        }

        //room service
        public async Task CreateRoom(CreateRoomRequest request)
        {
            var room = await _roomService.CreateRoomAsync();
            var roomData = await _roomService.JoinRoomAsync(room.RoomCode, request.PlayerName, Context.ConnectionId, request.PlayerGuid);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
        }

        public async Task JoinRoom(JoinRoomRequest request)
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

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomData = await _roomService.LeaveRoomAsync(Context.ConnectionId);

            if (roomData != null)
            {
                await Clients.All.SendAsync("RoomUpdated", roomData);
            }

            await base.OnDisconnectedAsync(exception);
        }


        //game service

        public async Task StartGame()
        {
            var room = await _gameService.StartGameAsync(Context.ConnectionId);

            var playerBoards = await _gameService.CreateBoardDTOsForPlayersInRoom(
                [.. room.Players.Select(p => p.Id)]
            );


            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);

            foreach (var entry in playerBoards)
            {
                var player = entry.Key;
                var boardDto = entry.Value;

                if (!string.IsNullOrEmpty(player.ConnectionId))
                {
                    await Clients.Client(player.ConnectionId).SendAsync("BoardUpdated", boardDto);
                }
            }
        }

        public async Task SubmitClues(SubmitCluesRequest request)
        {
            var room = await _gameService.SubmitCluesAsync(Context.ConnectionId, request.Words);
            var playerBoards = await _gameService.CreateBoardDTOsForPlayersInRoom(
                [.. room.Players.Select(p => p.Id)]
            );


            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);

            foreach (var entry in playerBoards)
            {
                var player = entry.Key;
                var boardDto = entry.Value;

                if (!string.IsNullOrEmpty(player.ConnectionId))
                {
                    await Clients.Client(player.ConnectionId).SendAsync("BoardUpdated", boardDto);
                }
            }
        }

    }
}

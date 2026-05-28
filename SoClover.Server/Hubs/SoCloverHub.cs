using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SoClover.Server.Models;
using SoClover.Server.Models.Requests;
using SoClover.Server.Services;

namespace SoClover.Server.Hubs
{
    [Authorize]
    public class SoCloverHub(IRoomService roomService, IGameService gameService) : Hub
    {
        private readonly IRoomService _roomService = roomService;
        private readonly IGameService _gameService = gameService;

        private Guid PlayerGuid
        {
            get
            {
                var idString = Context.UserIdentifier;
                if (string.IsNullOrEmpty(idString))
                {
                    throw new HubException("Player ID missing from token.");
                }
                return Guid.Parse(idString);
            }
        }

        private string PlayerName => Context.User?.Identity?.Name
            ?? throw new HubException("Username missing from token.");

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Player {PlayerName} ({PlayerGuid}) connected via WebSocket.");
            await base.OnConnectedAsync();
        }

        //room service
        public async Task CreateRoom(CreateRoomRequest request)
        {
            var room = await _roomService.CreateRoomAsync();
            var roomData = await _roomService.JoinRoomAsync(room.RoomCode, request.PlayerName, PlayerGuid);
            await Groups.AddToGroupAsync(Context.ConnectionId, room.RoomCode.ToUpper());
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
        }

        public async Task JoinRoom(JoinRoomRequest request)
        {
            var roomData = await _roomService.JoinRoomAsync(
                request.RoomCode,
                PlayerName,
                PlayerGuid
            );

            await Groups.AddToGroupAsync(Context.ConnectionId, roomData.RoomCode.ToUpper());
            await Clients.Group(roomData.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomData = await _roomService.LeaveRoomAsync(PlayerGuid);

            if (roomData != null && !string.IsNullOrEmpty(roomData.RoomCode))
            {
                await Clients.Group(roomData.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            }

            await base.OnDisconnectedAsync(exception);
        }


        //game service

        public async Task StartGame()
        {
            var room = await _gameService.StartGameAsync(PlayerGuid);
            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            await SendBoards(room.Id);
        }

        public async Task SubmitClues(SubmitCluesRequest request)
        {
            var room = await _gameService.SubmitCluesAsync(PlayerGuid, request.Words);
            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            await SendBoards(room.Id);
        }

        public async Task ReturnToWriting()
        {
            var room = await _gameService.ReturnToWritingAsync(PlayerGuid);
            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            await SendBoards(room.Id);
        }


        public async Task RotateCard(RotateCardRequest request)
        {
            var room = await _gameService.RotateCardAsync(request.GameRoomCardId);
            await SendBoards(room.Id);
        }

        public async Task MoveCardToSlot(MoveCardRequest request)
        {
            var room = await _gameService.MoveCardToSlotAsync(PlayerGuid, request.GameRoomCardId, request.PositionIndex);
            await SendBoards(room.Id);
        }

        public async Task Check()
        {
            var room = await _gameService.CheckAsync(PlayerGuid);
            await SendBoards(room.Id);
        }

        public async Task SendBoards(int roomId)
        {
            var playerBoards = await _gameService.CreateBoardDTOsForPlayersInRoomAsync(roomId);

            foreach (var entry in playerBoards)
            {
                var player = entry.Key;
                var boardDto = entry.Value;
                await Clients.User(player.PlayerGuid.ToString()).SendAsync("BoardUpdated", boardDto);
            }
        }
    }
}

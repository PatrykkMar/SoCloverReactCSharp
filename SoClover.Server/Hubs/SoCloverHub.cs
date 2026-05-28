using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SoClover.Server.Models;
using SoClover.Server.Models.Requests;
using SoClover.Server.Services;

namespace SoClover.Server.Hubs
{
    [Authorize]
    public class SoCloverHub(IRoomService roomService, IGameFlowService gameFlowService, IBoardService boardService) : Hub
    {
        private readonly IRoomService _roomService = roomService;
        private readonly IGameFlowService _gameFlowService = gameFlowService;
        private readonly IBoardService _boardService = boardService;

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


        //game flow service

        public async Task StartGame()
        {
            var room = await _gameFlowService.StartGameAsync(PlayerGuid);
            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            await SendBoards(room.Id);
        }

        public async Task SubmitClues(SubmitCluesRequest request)
        {
            var room = await _gameFlowService.SubmitCluesAsync(PlayerGuid, request.Words);
            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            await SendBoards(room.Id);
        }

        public async Task ReturnToWriting()
        {
            var room = await _gameFlowService.ReturnToWritingAsync(PlayerGuid);
            var roomData = await _roomService.GetRoomDataAsync(room.Id);
            await Clients.Group(room.RoomCode.ToUpper()).SendAsync("RoomUpdated", roomData);
            await SendBoards(room.Id);
        }

        //board service

        public async Task RotateCard(RotateCardRequest request)
        {
            var room = await _boardService.RotateCardAsync(request.GameRoomCardId);
            await SendBoards(room.Id);
        }

        public async Task MoveCardToSlot(MoveCardRequest request)
        {
            var room = await _boardService.MoveCardToSlotAsync(PlayerGuid, request.GameRoomCardId, request.PositionIndex);
            await SendBoards(room.Id);
        }

        public async Task Check()
        {
            var room = await _boardService.CheckAsync(PlayerGuid);
            await SendBoards(room.Id);
        }

        public async Task SendBoards(int roomId)
        {
            var playerBoards = await _boardService.CreateBoardDTOsForPlayersInRoomAsync(roomId);

            foreach (var entry in playerBoards)
            {
                var player = entry.Key;
                var boardDto = entry.Value;
                await Clients.User(player.PlayerGuid.ToString()).SendAsync("BoardUpdated", boardDto);
            }
        }
    }
}

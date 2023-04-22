using System.Linq.Expressions;
using System.Net.WebSockets;
namespace WebSocket_FIBRPN
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CinemaEndpoint _server;
        private List<WebSocket> OpenedSockets = new();
        public WebSocketMiddleware(RequestDelegate next, CinemaEndpoint server)
        {
            _next = next;
            _server = server;
        }
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await _server.Open(socket);
            OpenedSockets.Add(socket);
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    await HandleMessage(socket);
                }
            }
            catch (Exception ex)
            {
                OpenedSockets.Remove(socket);
                await _server.Close(socket);
                await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.Message, CancellationToken.None);
                throw;
            }
        }
        private async Task HandleMessage(WebSocket socket)
        {
            var request = await StringJsonEncoder.ReceiveAsync(socket);
            if (request.operation is not null)
            {
                Console.WriteLine($"Operation received: {request.operation}");
                switch (request.operation.Type)
                {
                    case Command.InitRoom:
                        await _server.InitRoom(socket, request.operation);
                        break;
                    case Command.GetRoomSize:
                        await StringJsonEncoder.SendAsync(socket, await _server.GetRoomSize());
                        break;
                    case Command.UpdateSeats:
                        await _server.UpdateSeats(socket);
                        break;
                    case Command.LockSeat:
                        var lockSeatStatus = await _server.LockSeat(socket, request.operation);
                        if (lockSeatStatus != null)
                        {
                            foreach (var openedSocket in OpenedSockets)
                            {
                                await StringJsonEncoder.SendAsync(openedSocket, lockSeatStatus);
                            }
                        }
                        break;
                    case Command.UnlockSeat:
                        var unlockSeatStatus = await _server.UnlockSeat(socket, request.operation);
                        if (unlockSeatStatus != null)
                        {
                            foreach (var openedSocket in OpenedSockets)
                            {
                                await StringJsonEncoder.SendAsync(openedSocket, unlockSeatStatus);
                            }
                        }
                        break;
                    case Command.ReserveSeat:
                        var reserveSeatStatus = await _server.ReserveSeat(socket, request.operation);
                        if (reserveSeatStatus != null)
                        {
                            foreach (var openedSocket in OpenedSockets)
                            {
                                await StringJsonEncoder.SendAsync(openedSocket, reserveSeatStatus);
                            }
                        }
                        break;
                    default:
                        var response = await _server.InvalidOperationMessage(socket, request.operation);
                        if (response is not null)
                        {
                            await StringJsonEncoder.SendAsync(socket, response);
                        }
                        break;
                }
            }
            else if (request.result.MessageType == WebSocketMessageType.Close)
            {
                OpenedSockets.Remove(socket);
                await _server.Close(socket);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }
    }
}
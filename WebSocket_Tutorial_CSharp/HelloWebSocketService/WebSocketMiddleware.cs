﻿using System.Net.WebSockets;
namespace HelloWebSocketService
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HelloEndpoint _server;
        public WebSocketMiddleware(RequestDelegate next, HelloEndpoint server)
        {
            _next = next;
            _server = server;
        }
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await _server.Open(socket);
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    await HandleMessage(socket);
                }
            }
            catch (Exception ex)
            {
                await _server.Close(socket);
                await socket.CloseAsync(WebSocketCloseStatus.InternalServerError,
                ex.Message, CancellationToken.None);
                throw;
            }
        }
        private async Task HandleMessage(WebSocket socket)
        {
            var request = await StringEncoder.ReceiveAsync(socket);
            if (request.message is not null)
            {
                var response = await _server.Message(socket, request.message);
                if (response is not null)
                {
                    await StringEncoder.SendAsync(socket, response);
                }
            }
            else if (request.result.MessageType == WebSocketMessageType.Close)
            {
                await _server.Close(socket);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                null, CancellationToken.None);
            }
        }
    }
}
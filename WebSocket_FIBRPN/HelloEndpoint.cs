using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
namespace WebSocket_FIBRPN
{
    [Route("WebSocket_FIBRPN/cinema")]
    public class HelloEndpoint
    {
        public async Task Open(WebSocket socket)
        {
            Console.WriteLine("WebSocket opened.");
        }

        public async Task Close(WebSocket socket)
        {
            Console.WriteLine("WebSocket closed.");
        }

        public async Task Error(WebSocket socket, Exception ex)
        {
            Console.WriteLine("WebSocket error: " + ex.Message);
        }

        public async Task<Operation> Message(WebSocket socket, Operation operation)
        {
            Console.WriteLine($"WebSocket message: {operation}");
            if (operation is not null)
            {
                await StringJsonEncoder.SendAsync(socket, operation);
            }
            return operation;
        }
    }
}
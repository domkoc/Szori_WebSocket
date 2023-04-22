using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebSocket_FIBRPN
{
    public class StringJsonEncoder
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters ={
                new JsonStringEnumConverter( JsonNamingPolicy.CamelCase)
            }
        };
        public static async Task<(WebSocketReceiveResult result, Operation? operation)> ReceiveAsync(WebSocket socket)
        {
            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(buffer: new
            ArraySegment<byte>(buffer), cancellationToken: CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                try
                {
                    Operation operation = JsonSerializer.Deserialize<Operation>(text, jsonSerializerOptions);
                    return (result, operation);
                } catch
                {
                    return (result, null);
                }
            }
            return (result, null);
        }
        public static async Task SendAsync(WebSocket socket, Operation operation)
        {
            var message = JsonSerializer.Serialize(operation, jsonSerializerOptions);
            var buffer = new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0,
            message.Length);
            await socket.SendAsync(buffer: buffer, messageType: WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
        }
    }
}
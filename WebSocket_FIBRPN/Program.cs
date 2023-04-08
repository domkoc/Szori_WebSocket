using Microsoft.AspNetCore.WebSockets;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

using WebSocket_FIBRPN;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<HelloEndpoint>();
var app = builder.Build();
app.UseStaticFiles();
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();
app.Run();
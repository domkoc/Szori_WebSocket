using WebSocket_FIBRPN;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CinemaEndpoint>();
var app = builder.Build();
app.UseStaticFiles();
app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();
app.Run();

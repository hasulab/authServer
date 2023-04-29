using AuthServer.Extentions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthServerServices();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseAuthServer();

app.Run();

using AuthServer.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthServerServices();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseAuthStaticFiles();
app.UseAuthServer();

app.Run();

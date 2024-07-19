using AuthServer.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthServerServices();
builder.Services.AddCors();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDevAuthHomepageEndpoint();
}
else
{
    app.MapGet("/", () => "Hello World!");
}

app.UseAuthStaticFiles();
app.UseAuthServer();
app.UseCors();

app.Run();


public partial class Program { }
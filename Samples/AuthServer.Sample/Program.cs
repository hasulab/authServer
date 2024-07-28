using AuthServer.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthServerServices();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDevAuthHomepageEndpoint();
    app.UseSwagger();
    app.UseSwaggerUI();
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
# authServer
## Auth server is a sample app purpose is to run auth on local for Dev and test for non production use only.
## Getting Started 

* Create a new ASP.NET Code empty Project
* Add latest [Hasulab.AuthServer.Simulator nuget library](https://www.nuget.org/packages/Hasulab.AuthServer.Simulator/) version reference to your proejct
* Add following lines to Progarm.cs
* Add ```builder.Services.AddAuthServerServices();```
```
using AuthServer.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthServerServices();
...
var app = builder.Build();
...
```
and add services 
```
...
app.UseAuthServer();
...

app.Run();
```

Please see the ```AuthServer.Sample``` project
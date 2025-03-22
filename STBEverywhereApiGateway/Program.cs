var builder = WebApplication.CreateBuilder(args);

// Ajouter les services YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Utiliser YARP
app.MapReverseProxy();

app.Run();
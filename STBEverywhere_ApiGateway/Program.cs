
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Activer les logs d?taill?s
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configuration de l'authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            NameClaimType = ClaimTypes.NameIdentifier, // Important
            RoleClaimType = ClaimTypes.Role // Si vous utilisez les rôles
        };
    });

builder.Services.AddAuthorization();

// Ajouter les services YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// Configuration Kestrel pour les requêtes volumineuses
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 5242880; // 5MB
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5242880; // 5MB
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (path.StartsWithSegments("/api/auth/login") || path.StartsWithSegments("/api/Client/register") || path.StartsWithSegments("/api/client/upload-documents") || path.StartsWithSegments("/api/compte/GetSoldeByRIB"))
    {
        // Ignorer l'authentification pour ces routes
        await next();
        return;
    }
  

    var endpoint = context.GetEndpoint();
    if (endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null)
    {
        if (!context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }

    await next();
});
app.UseStaticFiles();
app.UseRouting();
// Utiliser l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();

// Utiliser YARP
app.MapReverseProxy();
app.Run("http://localhost:5000"); // Changez le port ici
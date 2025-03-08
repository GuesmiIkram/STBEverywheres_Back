using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_back_APIChequier.Hubs;
using STBEverywhere_back_APIChequier.Services;
using FluentAssertions.Common;
using STBEverywhere_back_APIChequier.Jobs;
using STBEverywhere_back_APIChequier.Repository.IRepositoy;
using STBEverywhere_back_APIChequier.Repository;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Ajouter SignalR
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

// Configuration de la base de données
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseMySql(
      builder.Configuration.GetConnectionString("DefaultConnection"),
      ServerVersion.Parse("8.0.0-mysql") // Mets la version exacte de MySQL ici
  ));

// Enregistrement des services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ChequierService>();
builder.Services.AddScoped<IDemandesChequiersRepository, DemandesChequiersRepository>();
builder.Services.AddScoped<IEmailLogRepository, EmailLogRepository>();
builder.Services.AddScoped<IChequierRepository, ChequierRepository>();
//builder.Services.AddHostedService<ChequierJob>();
builder.Services.AddHostedService<EmailJob>();
builder.Services.AddHostedService<ChequierJob>();

builder.Services.AddHostedService<ChequierLivraisonJob>();

builder.Services.AddControllers().AddNewtonsoftJson();

// Configuration Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Configuration de l'authentification JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // À activer en production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization();
// Déplacer CORS avant `builder.Build()`**
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials()
                          .SetIsOriginAllowed(_ => true));
});

var app = builder.Build(); // Ici, les services deviennent en lecture seule !

// Appliquer les migrations automatiquement (optionnel)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    //context.Database.Migrate(); // Décommentez pour appliquer les migrations automatiquement
}

// Configurer le pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseCors("CorsPolicy"); //Utilisation de CORS après `builder.Build()`

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
//app.MapHub<NotificationHub>("/hubs/notificationHub");

app.Run();

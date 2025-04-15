using STBEverywhere_back_APIAgent.Service.IService;
using STBEverywhere_back_APIAgent.Service;
using STBEverywhere_back_APIAgent.Repository.IRepository;
using STBEverywhere_back_APIAgent.Repository;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using Microsoft.OpenApi.Models;
using STBEverywhere_ApiAuth.Repositories;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
var builder = WebApplication.CreateBuilder(args);

// Configuration de la base de donn�es MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.Parse("8.0.0-mysql")
    )
);
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IReclamationService, ReclamationService>();
builder.Services.AddScoped<IReclamationRepository, ReclamationRepository>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpClient("CompteService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5185");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf")); // Exemple avec Protobuf
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configuration de l'authentification JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // � activer en production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]
        };
    });

// Ajouter les services aux containers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configuration Swagger + s�curit� JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                      "Entrez 'Bearer' [espace] et ensuite votre token dans le champ ci-dessous. \r\n\r\n" +
                      "Exemple: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
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

// Injection de d�pendances
builder.Services.AddScoped<IDemandeModificationDecouvertRepository, DemandeModificationDecouvertRepository>();
builder.Services.AddScoped<IDemandeModificationDecouvertService, DemandeModificationDecouvertService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


// CORS pour permettre les requ�tes du frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Acc�s au contexte HTTP pour r�cup�rer les headers dans les controllers
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS activ�
app.UseCors("AllowAngularOrigins");
app.UseAuthentication(); // Activation correcte de l'authentification
app.UseAuthorization();

app.MapControllers();
app.Run();

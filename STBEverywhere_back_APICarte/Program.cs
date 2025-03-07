using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using STBEverywhere_Back_SharedModels.Models;

using STBEverywhere_back_APICarte.Repository;
using STBEverywhere_back_APICarte.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configuration de la base de donn�es
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseMySql(
      builder.Configuration.GetConnectionString("DefaultConnection"),
      ServerVersion.Parse("8.0.0-mysql") // Mets la version exacte de MySQL ici
  ));

// Enregistrement des repositories
builder.Services.AddScoped<ICarteRepository, CarteRepository>();
builder.Services.AddScoped<EmailService>();
// Enregistrement des services
builder.Services.AddScoped<ICarteService, CarteService>();
builder.Services.AddHostedService<CarteCreationJob>();
builder.Services.AddHostedService<CarteDisponibleJob>();
builder.Services.AddHostedService<CarteLivreeJob>();

builder.Services.AddControllers().AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configuration de Swagger avec JWT
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
        options.RequireHttpsMetadata = false; // � activer en production
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

// Configuration CORS pour Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowCredentials()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Appliquer les migrations automatiquement (optionnel)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    //context.Database.Migrate(); // D�commentez pour appliquer les migrations automatiquement
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseCors("AllowAngularOrigins");
app.UseAuthorization();

app.MapControllers();

app.Run();
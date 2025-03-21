using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace STBEverywhere_ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Vérifier si loginDto est null
            if (loginDto == null)
                return BadRequest("Invalid login request");

            // Authentifier l'utilisateur via le backend
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5001/api/auth/login", loginDto);

            if (!response.IsSuccessStatusCode)
                return Unauthorized("Invalid credentials");

            // Lire la réponse et générer le token
            var client = await response.Content.ReadFromJsonAsync<Client>();
            if (client == null)
                return Unauthorized("Invalid response from authentication service");

            var token = GenerateJwtToken(client);

            // Retourner le token au client
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(Client client)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
                new Claim(ClaimTypes.Email, client.Email)
            };

            // Récupérer la clé secrète depuis appsettings.json
            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                throw new InvalidOperationException("JWT Secret Key is missing in configuration");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

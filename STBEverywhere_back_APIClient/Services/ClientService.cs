using STBEverywhere_back_APIClient.Repositories;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;
using System.Net.Http;
using STBEverywhere_ApiAuth.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace STBEverywhere_back_APIClient.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientService(
            IUserRepository userRepository,
            IClientRepository clientRepository,
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _clientRepository = clientRepository;
            _httpClient = httpClient;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<Client> GetClientByIdAsync(int clientId)
        {
            return await _clientRepository.GetClientByIdAsync(clientId);
        }

        public async Task<bool> UpdateClientInfoAsync(int clientId, Client updatedClient)
        {
            var existingClient = await _clientRepository.GetClientByIdAsync(clientId);
            if (existingClient == null)
                return false;



            existingClient.Telephone = updatedClient.Telephone;
            existingClient.Email = updatedClient.Email;
            existingClient.Adresse = updatedClient.Adresse;
            existingClient.Civilite = updatedClient.Civilite;
            existingClient.EtatCivil = updatedClient.EtatCivil;
            existingClient.Residence = updatedClient.Residence;


            await _clientRepository.UpdateClientAsync(existingClient);
            return true;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Vérifier si le RIB est valide et appartient à l'email donné
            var compte = await GetCompteByRIBAsync(registerDto.RIB);
            if (compte == null)
                throw new InvalidOperationException("Le RIB est invalide ou n'existe pas.");

            var client = await _clientRepository.GetClientByIdAsync(compte.ClientId);
            if (client == null || client.Email != registerDto.Email)
                throw new InvalidOperationException("Le RIB ne correspond pas à l'email fourni.");

            // 2. Vérifier si l'utilisateur existe déjà dans la table User
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return "Utilisateur déjà inscrit.";
            }

            // 3. Créer un nouvel utilisateur et crypter son mot de passe
            var newUser = new User
            {
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = UserRole.Client
            };

            await _userRepository.AddAsync(newUser);

            // 4. Lier l'utilisateur au client dans la table Client
            client.UserId = newUser.Id;
            await _clientRepository.UpdateClientAsync(client);

            return "Inscription réussie.";
        }

        // Méthode pour récupérer le compte par RIB via API externe
        private async Task<Compte?> GetCompteByRIBAsync(string rib)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5185/api/compte/GetByRIB/{rib}");

            if (!response.IsSuccessStatusCode)
                return null;

            var comptes = await response.Content.ReadFromJsonAsync<List<Compte>>();
            return comptes?.FirstOrDefault();
        }



    }
}

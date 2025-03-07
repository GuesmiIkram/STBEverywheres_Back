


using STBEverywhere_back_APIClient.Repositories;
using STBEverywhere_Back_SharedModels;
namespace STBEverywhere_back_APIClient.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
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



    }
}

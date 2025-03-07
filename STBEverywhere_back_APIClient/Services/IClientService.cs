using STBEverywhere_Back_SharedModels;



namespace STBEverywhere_back_APIClient.Services
{
    public interface IClientService
    {
        Task<Client> GetClientByIdAsync(int clientId);
        Task<bool> UpdateClientInfoAsync(int clientId, Client updatedClient);
    }
}

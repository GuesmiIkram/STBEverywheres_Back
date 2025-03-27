
using STBEverywhere_Back_SharedModels;
namespace STBEverywhere_back_APIClient.Repositories
{
    public interface IClientRepository
    {
        Task AddClientAsync(Client client);
        Task<Client> GetClientByNumCinAsync(string numCin);
        Task<Client> GetClientByEmailAsync(string email);
        Task<Client> GetClientByIdAsync(int id);
        Task<Client> GetClientByUserIdAsync(int userId);
        Task UpdateClientAsync(Client client);
       
      
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}

using STBEverywhere_Back_SharedModels.Data;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels;
namespace STBEverywhere_back_APIClient.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Client> GetClientByNumCinAsync(string numCin)
        {
            return await _context.Clients
                                 .FirstOrDefaultAsync(c => c.NumCIN == numCin);
        }
        public Client GetClientByEmail(string email)
        {
            return _context.Clients.AsNoTracking().FirstOrDefault(c => c.Email == email);
        }
        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }
        public Client GetClientById(int id) // Implémentez cette méthode
        {
            return _context.Clients.FirstOrDefault(c => c.Id == id);
        }

    } 
}

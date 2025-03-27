using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STBEverywhere_back_APIClient.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddClientAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
        }

        public async Task<Client> GetClientByNumCinAsync(string numCin)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.NumCIN == numCin);
        }

        public async Task<Client> GetClientByEmailAsync(string email)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Client> GetClientByUserIdAsync(int userId)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

 
        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients
                .Include(c => c.User)
                .ToListAsync();
        }
    }
}
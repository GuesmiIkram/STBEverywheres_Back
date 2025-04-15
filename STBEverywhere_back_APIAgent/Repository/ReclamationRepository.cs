using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIAgent.Repository.IRepository;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIAgent.Repository
{
    public class ReclamationRepository : IReclamationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReclamationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reclamation> GetByIdAsync(int id)
        {
            return await _context.Reclamations
                .Include(r => r.Client) // Pour accéder à l'email du client
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(Reclamation reclamation)
        {
            _context.Reclamations.Update(reclamation);
            await _context.SaveChangesAsync();
        }
    }


}

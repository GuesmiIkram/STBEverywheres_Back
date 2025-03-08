using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIChequier.Repository.IRepositoy;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIChequier.Repository
{
    public class DemandesChequiersRepository : IDemandesChequiersRepository
    {

        private readonly ApplicationDbContext _context;

        public DemandesChequiersRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetRibComptesByClientId(int clientId)
        {
            return await _context.Comptes
                .Where(c => c.ClientId == clientId)
                .Select(c => c.RIB)
                .ToListAsync();
        }

        public async Task<List<DemandeChequier>> GetDemandesByRibComptes(List<string> ribComptes)
        {
            return await _context.DemandesChequiers
                .Where(d => ribComptes.Contains(d.RibCompte))
                .ToListAsync();
        }

        public async Task<IEnumerable<DemandeChequier>> GetAllAsync()
        {
            return await _context.DemandesChequiers.ToListAsync();
        }

        public async Task<DemandeChequier?> GetByIdAsync(int id)
        {
            return await _context.DemandesChequiers
                .Include(d => d.Feuilles) // Si les feuilles doivent être incluses
                .FirstOrDefaultAsync(d => d.IdDemande == id);
        }
        
        public async Task AddAsync(DemandeChequier demande)
        {
            await _context.DemandesChequiers.AddAsync(demande);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DemandeChequier demande)
        {
            _context.DemandesChequiers.Update(demande);
            await _context.SaveChangesAsync();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var demande = await _context.DemandesChequiers.FindAsync(id);
            if (demande != null)
            {
                _context.DemandesChequiers.Remove(demande);
                await _context.SaveChangesAsync();
            }
        }
    }
}


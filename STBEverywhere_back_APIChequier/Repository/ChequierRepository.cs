using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIChequier.Repository.IRepositoy;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIChequier.Repository
{
    public class ChequierRepository : IChequierRepository
    {
        private readonly ApplicationDbContext _context;

        public ChequierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Chequier>> GetChequiersByDemandesIds(List<int> demandesIds)
        {
            return await _context.Chequiers
                .Where(c => demandesIds.Contains(c.DemandeChequierId))
                .ToListAsync();
        }
        public async Task<List<DemandeChequier>> GetDemandesByRibComptes(List<string> ribComptes)
        {
            return await _context.DemandesChequiers
                .Where(d => ribComptes.Contains(d.RibCompte))
                .ToListAsync();
        }
        public async Task<List<DemandeChequier>> GetChequiersDisponiblesAsync()
        {
            return await _context.DemandesChequiers
                .Where(c => c.Status == DemandeStatus.DisponibleEnAgence)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

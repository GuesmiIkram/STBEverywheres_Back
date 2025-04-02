using STBEverywhere_back_APICompte.Repository.IRepository;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;
using Microsoft.EntityFrameworkCore;

namespace STBEverywhere_back_APICompte.Repository
{
    public class FraisCompteRepository : IFraisCompteRepository
    {
        private readonly ApplicationDbContext _context;

        public FraisCompteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FraisCompte>> GetAllAsync()
        {
            return await _context.FraisComptes.ToListAsync();
        }

        public async Task<FraisCompte> GetByIdAsync(int id)
        {
            return await _context.FraisComptes.FindAsync(id);
        }

        public async Task CreateAsync(FraisCompte fraisCompte)
        {
            await _context.FraisComptes.AddAsync(fraisCompte);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FraisCompte fraisCompte)
        {
            _context.FraisComptes.Update(fraisCompte);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var fraisCompte = await _context.FraisComptes.FindAsync(id);
            if (fraisCompte != null)
            {
                _context.FraisComptes.Remove(fraisCompte);
                await _context.SaveChangesAsync();
            }
        }

    }
}

using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;
using System.Linq.Expressions;

namespace STBEverywhere_back_APIClient.Repositories
{
    public class BeneficiaireRepository:IBeneficiaireRepository
    {

        private readonly ApplicationDbContext _context;

        public BeneficiaireRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Beneficiaire?> GetByIdAsync(int id)
        {
           
            return await _context.Beneficiaires
                .FirstOrDefaultAsync(b => b.Id == id);
        }



        public async Task<IEnumerable<Beneficiaire>> GetAllAsync()
        {
            return await _context.Beneficiaires.ToListAsync();
        }

        public async Task<IEnumerable<Beneficiaire>> GetAllAsync(Expression<Func<Beneficiaire, bool>> predicate)
        {
            return await _context.Beneficiaires.Where(predicate).ToListAsync();
        }


        public async Task CreateAsync(Beneficiaire beneficiaire)
        {
            await _context.Beneficiaires.AddAsync(beneficiaire);
            await _context.SaveChangesAsync();
        }

    }
}

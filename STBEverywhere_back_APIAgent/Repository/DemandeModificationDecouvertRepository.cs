using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIAgent.Repository.IRepository;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIAgent.Repository
{
    public class DemandeModificationDecouvertRepository : IDemandeModificationDecouvertRepository
    {
        private readonly ApplicationDbContext _context;

        public DemandeModificationDecouvertRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DemandeModificationDecouvert?> GetByIdAsync(int id)
        {
            return await _context.DemandeModificationDecouverts.FindAsync(id);
        }

        public async Task UpdateAsync(DemandeModificationDecouvert demande)
        {
            _context.DemandeModificationDecouverts.Update(demande);
            await _context.SaveChangesAsync();
        }


        public async Task<List<DemandeModificationDecouvert>> GetByStatutAsync(string statut)
        {
            if (!Enum.TryParse<StatutDemandeEnum>(statut, out var statutEnum))
            {
                throw new ArgumentException("Statut invalide.", nameof(statut));
            }

            return await _context.DemandeModificationDecouverts
                .Where(d => d.StatutDemande == statutEnum)
                .ToListAsync();
        }


    }
}

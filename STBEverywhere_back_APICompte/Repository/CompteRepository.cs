using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_back_APICompte.Repository.IRepository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APICompte.Repository
{
    public class CompteRepository : Repository<Compte>, ICompteRepository
    {
        private readonly ApplicationDbContext _db;

        public CompteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task CreateDemandeModificationAsync(DemandeModificationDecouvert demande)
        {
            await _db.DemandeModificationDecouverts.AddAsync(demande);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsByRibAsync(string rib)
        {
            return await _db.Comptes.AnyAsync(c => c.RIB == rib);
        }

        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(string ribCompte, string statut)
        {
            if (!Enum.TryParse<StatutDemandeEnum>(statut, out var statutEnum))
                throw new ArgumentException("Statut de demande invalide", nameof(statut));

            return await _db.DemandeModificationDecouverts
                .Where(d => d.RIBCompte == ribCompte && d.StatutDemande == statutEnum)
                .ToListAsync();
        }

        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationByCompteRibAsync(string rib)
        {
            return await _db.DemandeModificationDecouverts
                .Where(d => d.RIBCompte == rib)
                .ToListAsync();
        }


        /* public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(string ribCompte, string statut)
         {
             return await _db.DemandeModificationDecouverts
                 .Where(d => d.RIBCompte == ribCompte && d.StatutDemande == statut)
                 .ToListAsync();
         }*/



        public async Task<IEnumerable<DemandeModificationDecouvert>> GetDemandesModificationAsync(List<string> ribComptes)
        {
            return await _db.DemandeModificationDecouverts
                                 .Where(d => ribComptes.Contains(d.RIBCompte))
                                 .ToListAsync();
        }





        public async Task<decimal> GetSoldeByRIBAsync(string rib)
        {
            // Récupérer le compte par son RIB
            var compte = await _db.Comptes
                .FirstOrDefaultAsync(c => c.RIB == rib);

            if (compte == null)
            {
                throw new InvalidOperationException("Compte introuvable.");
            }

            // Retourner le solde du compte
            return compte.Solde;
        }

        public async Task CreateAsync(Compte entity)
        {
            await _db.Comptes.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<Compte> GetByRibAsync(string rib)
        {
            return await _db.Comptes
                .FirstOrDefaultAsync(c => c.RIB == rib);
        }

        public async Task<Compte> UpdateAsync(Compte entity)
        {
            _db.Comptes.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<List<Compte>> GetAllAsync(Expression<Func<Compte, bool>> filter = null)
        {
            IQueryable<Compte> query = _db.Comptes;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<int?> GetClientIdByRibAsync(string rib)
        {
            var compte = await _db.Comptes
                .Include(c => c.Client) // Assurez-vous que la relation Compte-Client est configurée
                .FirstOrDefaultAsync(c => c.RIB == rib);

            return compte?.Client?.Id; // Retourne l'ID du client ou null si non trouvé
        }

        public async Task<Compte> GetCompteByRIBAsync(string rib)
        {
            return await _db.Comptes
                .Include(c => c.Client) // Inclure le client associé
                .FirstOrDefaultAsync(c => c.RIB == rib);
        }
    }
}
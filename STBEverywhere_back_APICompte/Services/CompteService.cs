using STBEverywhere_back_APICompte.Repository.IRepository;
using STBEverywhere_Back_SharedModels;
using System.Linq.Expressions;
namespace STBEverywhere_back_APICompte.Services
{
    public class CompteService : ICompteService
    {

        private readonly ICompteRepository _compteRepository;

        public CompteService(ICompteRepository compteRepository)
        {
            _compteRepository = compteRepository;
        }

        public async Task<List<Compte>> GetAllAsync(Expression<Func<Compte, bool>> filter = null)
        {
            return await _compteRepository.GetAllAsync(filter);
        }

        public async Task<Compte> GetByRibAsync(string rib)
        {
            return await _compteRepository.GetByRibAsync(rib);
        }

        public async Task<Compte> UpdateAsync(Compte entity)
        {
            return await _compteRepository.UpdateAsync(entity);
        }

        public async Task<Compte> CreateAsync(Compte entity)
        {
            await _compteRepository.CreateAsync(entity);
            return entity;
        }
        public async Task<Client> GetClientByRIBAsync(string rib)
        {
            var compte = await _compteRepository.GetCompteByRIBAsync(rib);
            if (compte == null)
                return null;

            return compte.Client; // Retourne le client associé au compte
        }
        public async Task<decimal> GetSoldeByRIBAsync(string rib)
        {
            var compte = await _compteRepository.GetByRibAsync(rib);
            if (compte == null)
            {
                throw new InvalidOperationException("Compte introuvable.");
            }

            return compte.Solde;
        }

        public async Task SaveAsync()
        {
            await _compteRepository.SaveAsync();
        }
        string ICompteService.GenerateUniqueRIB()
        {
            string guidString = Guid.NewGuid().ToString("N");

            // Extrait uniquement les chiffres
            string ribDigits = string.Concat(guidString.Where(c => char.IsDigit(c)));

            // Vérifie que la chaîne a bien au moins 20 chiffres
            if (ribDigits.Length < 20)
            {
                ribDigits = ribDigits.PadRight(20, '0'); // Complète avec des zéros si nécessaire
            }

            // Retourne les 20 premiers chiffres
            return ribDigits.Substring(0, 20);
        }
    }
}


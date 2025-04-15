using STBEverywhere_back_APIAgent.Repository.IRepository;
using STBEverywhere_back_APIAgent.Service.IService;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIAgent.Service
{
    public class ReclamationService : IReclamationService
    {
        private readonly IReclamationRepository _reclamationRepository;
        private readonly EmailService _emailService;

        public ReclamationService(IReclamationRepository reclamationRepository, EmailService emailService)
        {
            _reclamationRepository = reclamationRepository;
            _emailService = emailService;
        }

        public async Task<bool> RepondreAReclamationAsync(int reclamationId, string contenuReponse, int idAgent)
        {
            var reclamation = await _reclamationRepository.GetByIdAsync(reclamationId);
            if (reclamation == null || reclamation.Statut == ReclamationStatut.traite)
                return false;

            reclamation.Reponse = contenuReponse;
            reclamation.IdAgent = idAgent;
            reclamation.DateResolution = DateTime.UtcNow;
            reclamation.Statut = ReclamationStatut.traite;

            await _reclamationRepository.UpdateAsync(reclamation);

            var email = reclamation.Client?.Email;
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _emailService.SendEmailAsync(email, "Réponse à votre réclamation", contenuReponse);
        }
    }

}

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIChequier.Controllers;
using STBEverywhere_back_APIChequier.Hubs;
using STBEverywhere_back_APIChequier.Repository.IRepositoy;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIChequier.Services
{
    public class ChequierService
    {

        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<ChequierService> _logger;
        private readonly IChequierRepository _chequierRepository;
        private readonly IEmailLogRepository _emailLogRepository;
        public ChequierService(IChequierRepository chequierRepository,IEmailLogRepository emailLogRepository, ILogger<ChequierService> logger, ApplicationDbContext context, EmailService emailService, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _emailService = emailService;
            _hubContext = hubContext;
            _logger = logger;
            _chequierRepository = chequierRepository ?? throw new ArgumentNullException(nameof(chequierRepository));
            _emailLogRepository = emailLogRepository ?? throw new ArgumentNullException(nameof(emailLogRepository));

        }

        /// <summary>
        /// Vérifie si des chéquiers doivent être marqués comme "Livré" et les ajoute en base.
        /// </summary>
        public async Task VérifierChéquiersDisponibles()
        {
            /*var chequiers = await _context.DemandesChequiers
               
                .Where(c => c.Status == DemandeStatus.DisponibleEnAgence)
                .ToListAsync();*/
            var chequiers = await _chequierRepository.GetChequiersDisponiblesAsync() ?? new List<DemandeChequier>();

            //var chequiers = await _chequierRepository.GetChequiersDisponiblesAsync();
            foreach (var chequier in chequiers)
            {
                //chequier.DateLivraison = DateTime.Now;
                //chequier.Status = ChequierStatus.Active;

                // Sauvegarder les changements dans la base de données
                //await _context.SaveChangesAsync();

                // Envoi d'un email au client
                /*await _emailService.SendEmailAsync(chequier.Email, "Votre chéquier est disponible",
                    "Votre chéquier est prêt à être récupéré en agence.");*/
                /*await _emailService.LogEmailAsync(chequier.Email, "Votre chéquier est disponible",
    "Votre chéquier est prêt à être récupéré en agence.",chequier.IdDemande);*/
                /*var existingEmailLog = await _context.EmailLogs
    .Where(e => e.DemandeId == chequier.IdDemande && e.IsEnvoye && e.EmailType== "disponibile en agence")
    .FirstOrDefaultAsync();*/
                var existingEmailLog = await _emailLogRepository.GetExistingEmailLogAsync(chequier.IdDemande,"disponibile en agence");

                if (existingEmailLog == null) // Pas encore envoyé
                {
                    await _emailService.LogEmailAsync(chequier.Email, "Votre chéquier est disponible",
                        "Votre chéquier est prêt à être récupéré en agence.", chequier.IdDemande, "disponibile en agence");
                }

                _logger.LogInformation("Envoi de notification pour le chéquier {ChequierId} à l'email {Email}.", chequier.NumeroChequier, chequier.Email);

                // Envoyer une notification en temps réel avec SignalR
                await _hubContext.Clients.User(chequier.Email)
                    .SendAsync("ReceiveNotification", "Votre chéquier est disponible en agence.");
            }
        }
    }
    }

using Microsoft.AspNetCore.SignalR;
using STBEverywhere_back_APIChequier.Hubs;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIChequier.Services;


namespace STBEverywhere_back_APIChequier.Jobs
{
    public class ChequierEnvoiRecommandeJob : BackgroundService
    {



        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ChequierDisponibleEnAgenceJob> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ChequierEnvoiRecommandeJob(IServiceProvider serviceProvider, ILogger<ChequierDisponibleEnAgenceJob> logger, IHubContext<NotificationHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _hubContext = hubContext;
            ;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                        // Récupérer les demandes dont le statut est 'expedié'
                        var demandesDispo = await context.DemandesChequiers
                            .Where(d => d.Status == DemandeStatus.expédié && d.ModeLivraison == ModeLivraison.EnvoiRecommande) 
                            .ToListAsync();

                        foreach (var demande in demandesDispo) // Ajout de la boucle foreach
                        {
                            // Vérifier si un chéquier existe déjà pour cette demande
                            var existingChequier = await context.Chequiers
                                .FirstOrDefaultAsync(c => c.DemandeChequierId == demande.IdDemande);

                            if (existingChequier == null) // Si aucun chéquier n'existe pour cette demande
                            {
                                var chequier = new Chequier
                                {
                                    DemandeChequierId = demande.IdDemande,
                                    Status = ChequierStatus.Active,
                                    DateLivraison = DateTime.Now, // Marquer la date de livraison
                                };

                                // Ajouter le chéquier à la table
                                context.Chequiers.Add(chequier);
                                await context.SaveChangesAsync();

                                var existingEmailLog = await context.EmailLogs
                                    .Where(e => e.DemandeId == chequier.DemandeChequierId && e.IsEnvoye && e.EmailType == "cheque livre")
                                    .FirstOrDefaultAsync();

                                if (existingEmailLog == null) // Si l'email n'a pas encore été envoyé
                                {
                                    var contenu = $"Votre chéquier {demande.NumeroChequier} a été envoyé par courrier recommandé. Vous pouvez suivre la livraison à l'aide du numéro de suivi.\r\n\r\nCordialement,\r\nSTB";
                                    await emailService.LogEmailAsync(demande.Email, "Votre chéquier a été envoyé", contenu, demande.IdDemande, "cheque recommande");
                                }
                                _logger.LogInformation("Envoi de notification pour le chéquier {ChequierId} envoyé par recommandé à l'email {Email}.", demande.IdDemande, demande.Email);
                                await _hubContext.Clients.User(demande.Email).SendAsync("ReceiveNotification", $"Votre chéquier {demande.NumeroChequier} a été envoyé par courrier recommandé.");
                            }
                            else
                            {
                                _logger.LogInformation("Le chéquier pour la demande {DemandeId} existe déjà. Aucun ajout effectué.", demande.IdDemande);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la vérification des demandes de chéquiers livrés.");
                }

                // Vérifier toutes les minutes (ou ajuster la fréquence selon votre besoin)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}





using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APIChequier.Hubs;
using STBEverywhere_back_APIChequier.Services;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIChequier.Jobs
{
    public class ChequierLivraisonJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ChequierLivraisonJob> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ChequierLivraisonJob(IServiceProvider serviceProvider, ILogger<ChequierLivraisonJob> logger, IHubContext<NotificationHub> hubContext)
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

                        // Récupérer les demandes dont le statut est 'livré'
                        var demandesLivrees = await context.DemandesChequiers
                            .Where(d => d.Status == DemandeStatus.Livre) // Le statut doit être 'Livré'
                            .ToListAsync();

                        foreach (var demande in demandesLivrees) // Ajout de la boucle foreach
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
                                    var contenu = $"Le numéro de votre chéquier est {demande.NumeroChequier}.";
                                    await emailService.LogEmailAsync(demande.Email, "Votre chéquier est livré", contenu, demande.IdDemande, "cheque livre");
                                }

                                _logger.LogInformation("Envoi de notification pour le chéquier {ChequierId} à l'email {Email}.", chequier.Id, demande.Email);
                                await _hubContext.Clients.User(demande.Email).SendAsync("ReceiveNotification", $"Votre chéquier est livré. Le numéro de votre chéquier est {demande.NumeroChequier}.");
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

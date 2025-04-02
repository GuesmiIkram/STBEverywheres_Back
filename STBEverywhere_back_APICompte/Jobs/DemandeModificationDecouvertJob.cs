namespace STBEverywhere_back_APICompte.Jobs
{


    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using STBEverywhere_Back_SharedModels;
    using STBEverywhere_Back_SharedModels.Models;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using STBEverywhere_Back_SharedModels.Data;
    using STBEverywhere_back_APICompte.Services;

    public class DemandeModificationDecouvertJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DemandeModificationDecouvertJob> _logger;
        //private readonly IHubContext<NotificationHub> _hubContext;

        public DemandeModificationDecouvertJob(IServiceProvider serviceProvider, ILogger<DemandeModificationDecouvertJob> logger /*,IHubContext<NotificationHub> hubContext*/)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            //_hubContext = hubContext;
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

                        // Récupérer les demandes qui sont passées de "En attente" à "Accepté" ou "Refusé"
                        var demandesModifiees = await context.DemandeModificationDecouverts
                            .Where(d => d.StatutDemande != "En attente" && !d.MailEnvoyee) // Vérifier MailEnvoyee
                            .ToListAsync();

                        foreach (var demande in demandesModifiees)
                        {
                            var compte = await context.Comptes
                                .Include(c => c.Client) // Charger le client
                                .FirstOrDefaultAsync(c => c.RIB == demande.RIBCompte);

                            if (compte == null || compte.Client == null)
                            {
                                _logger.LogError("Aucun compte ou client associé trouvé pour le RIB {RIB}.", demande.RIBCompte);
                                continue;
                            }

                            string emailSubject;
                            string emailContent;

                            if (demande.StatutDemande == "Accepte")
                            {
                                // Mettre à jour le découvert autorisé
                                compte.DecouvertAutorise = demande.DecouvertDemande;
                                emailSubject = "Votre demande de modification de découvert a été acceptée";
                                emailContent = $"Bonjour,\n\nVotre demande de modification de découvert a été acceptée. \nVotre nouveau découvert autorisé est de {demande.DecouvertDemande}.\n\nCordialement,\nSTB";

                                _logger.LogInformation("Découvert mis à jour pour le compte {RIB} : {Decouvert}.", demande.RIBCompte, demande.DecouvertDemande);
                            }
                            else if (demande.StatutDemande == "Refuse")
                            {
                                emailSubject = "Votre demande de modification de découvert a été refusée";
                                emailContent = $"Bonjour,\n\nVotre demande de modification de découvert a été refusée.\nPour plus d'informations, veuillez contacter votre agence.\n\nCordialement,\nSTB";
                            }
                            else
                            {
                                _logger.LogWarning("Statut inconnu détecté : {Statut} pour la demande {Id}.", demande.StatutDemande, demande.Id);
                                continue; // Si le statut est invalide, on ignore cette demande
                            }

                            // Envoyer l'email
                            await emailService.SendEmailAsync(compte.Client.Email, emailSubject, emailContent);

                            // Marquer l'email comme envoyé
                            demande.MailEnvoyee = true;
                            await context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du traitement des demandes de modification de découvert.");
                }

                // Vérifier toutes les minutes
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }





    }


}

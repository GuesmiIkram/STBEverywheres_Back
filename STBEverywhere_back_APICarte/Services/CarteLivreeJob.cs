using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace STBEverywhere_back_APICarte.Services
{
    public class CarteLivreeJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CarteLivreeJob> _logger;

        public CarteLivreeJob(IServiceProvider serviceProvider, ILogger<CarteLivreeJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CarteLivreeJob démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var carteService = scope.ServiceProvider.GetRequiredService<ICarteService>();

                        // Récupérer les demandes avec le statut "Livrée" et où l'email n'a pas encore été envoyé
                        var demandesLivrees = await carteService.GetDemandesByStatutAsync("Livree");

                        _logger.LogInformation("Nombre de demandes livrées : {Count}", demandesLivrees.Count());

                        foreach (var demande in demandesLivrees)
                        {
                            // Vérifier si l'email a déjà été envoyé
                            if (demande.EmailEnvoyeLivree)
                            {
                                _logger.LogInformation("Email déjà envoyé pour la demande : {DemandeId}", demande.Iddemande);
                                continue; // Passer à la demande suivante
                            }

                            // Vérifier si l'email est à false
                            if (!demande.EmailEnvoyeLivree)
                            {
                                _logger.LogInformation("Envoi d'email pour la demande : {DemandeId}", demande.Iddemande);

                                // Envoyer l'email
                                await carteService.SendEmailAsync(
                                    demande.Email,
                                    "Votre carte est livrée",
                                    $"Votre carte {demande.NomCarte} a été livrée avec succès."
                                );

                                // Mettre à jour le champ EmailEnvoyeLivree
                                await carteService.UpdateEmailEnvoyeLivreeAsync(demande.Iddemande, true);

                                _logger.LogInformation("Email envoyé pour la demande : {DemandeId}", demande.Iddemande);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Une erreur s'est produite lors de l'exécution du job.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
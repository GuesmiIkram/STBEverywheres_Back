using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace STBEverywhere_back_APICarte.Services
{
    public class CarteCreationJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CarteCreationJob> _logger;

        public CarteCreationJob(IServiceProvider serviceProvider, ILogger<CarteCreationJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CarteCreationJob démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Créer un scope manuel pour accéder aux services scoped
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var carteService = scope.ServiceProvider.GetRequiredService<ICarteService>();

                        // Récupérer toutes les demandes avec le statut "Récupérée"
                        var demandesRecuperees = await carteService.GetDemandesByStatutAsync("Recuperee");

                        _logger.LogInformation("Nombre de demandes récupérées : {Count}", demandesRecuperees.Count());

                        foreach (var demande in demandesRecuperees)
                        {
                            _logger.LogInformation("Traitement de la demande : {DemandeId}", demande.Iddemande);

                            // Vérifier si une carte a déjà été ajoutée pour cette demande
                            if (demande.CarteAjouter)
                            {
                                _logger.LogInformation("Une carte a déjà été ajoutée pour la demande : {DemandeId}", demande.Iddemande);
                                continue; // Passer à la demande suivante
                            }

                            // Créer la carte pour cette demande
                            var carteCreee = await carteService.CreateCarteIfDemandeRecupereeAsync(demande.Iddemande);

                            if (carteCreee)
                            {
                                // Mettre à jour le champ CarteAjouter à true
                                demande.CarteAjouter = true;
                                await carteService.UpdateDemandeAsync(demande); // Mettre à jour la demande dans la base de données

                                _logger.LogInformation("Carte créée et demande mise à jour pour : {DemandeId}", demande.Iddemande);
                            }
                            else
                            {
                                _logger.LogWarning("Échec de la création de la carte pour la demande : {DemandeId}", demande.Iddemande);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Une erreur s'est produite lors de l'exécution du job.");
                }

                // Attendre avant la prochaine exécution (par exemple, toutes les 5 minutes)
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
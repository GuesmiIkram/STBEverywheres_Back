using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using STBEverywhere_Back_SharedModels.Models.enums;

namespace STBEverywhere_back_APICarte.Services
{
    public class CarteDisponibleJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CarteDisponibleJob> _logger;

        public CarteDisponibleJob(IServiceProvider serviceProvider, ILogger<CarteDisponibleJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CarteDisponibleJob démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var carteService = scope.ServiceProvider.GetRequiredService<ICarteService>();

                        var demandesDisponibles = await carteService.GetDemandesByStatutAsync(StatutDemande.DisponibleEnAgence);

                        _logger.LogInformation("Nombre de demandes disponibles à l'agence : {Count}", demandesDisponibles.Count());

                        foreach (var demande in demandesDisponibles)
                        {
                            // Vérifier si l'email a déjà été envoyé
                            if (demande.EmailEnvoye)
                            {
                                _logger.LogInformation("Email déjà envoyé pour la demande : {DemandeId}", demande.Iddemande);
                                continue; // Passer à la demande suivante
                            }

                            // Vérifier si l'email est à false
                            if (!demande.EmailEnvoye)
                            {
                                _logger.LogInformation("Envoi d'email pour la demande : {DemandeId}", demande.Iddemande);

                                // Envoyer l'email
                                await carteService.SendEmailAsync(
                                    demande.Email,
                                    "Votre carte est disponible à l'agence",
                                    $"Votre carte {demande.NomCarte} est disponible à l'agence."
                                );

                                // Mettre à jour le champ EmailEnvoye
                                await carteService.UpdateEmailEnvoyeAsync(demande.Iddemande, true);

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
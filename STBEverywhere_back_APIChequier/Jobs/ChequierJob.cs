using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using STBEverywhere_back_APIChequier.Services;

namespace STBEverywhere_back_APIChequier.Jobs
{
    public class ChequierJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ChequierJob> _logger;

        public ChequierJob(IServiceProvider serviceProvider, ILogger<ChequierJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ChequierJob démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var chequierService = scope.ServiceProvider.GetRequiredService<ChequierService>();

                        await chequierService.VérifierChéquiersDisponibles();

                        _logger.LogInformation("Vérification des chequiers terminée.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Une erreur s'est produite lors de l'exécution du job.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}

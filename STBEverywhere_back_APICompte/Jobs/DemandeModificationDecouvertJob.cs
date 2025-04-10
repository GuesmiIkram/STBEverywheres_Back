namespace STBEverywhere_back_APICompte.Jobs
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using STBEverywhere_Back_SharedModels.Data;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using STBEverywhere_back_APICompte.Services;
    using STBEverywhere_Back_SharedModels.Models;

    public class DemandeModificationDecouvertJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DemandeModificationDecouvertJob> _logger;

        public DemandeModificationDecouvertJob(
            IServiceProvider serviceProvider,
            ILogger<DemandeModificationDecouvertJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
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

                        var demandesModifiees = await context.DemandeModificationDecouverts
                            .Where(d => d.StatutDemande != StatutDemandeEnum.EnAttente && !d.MailEnvoyee)
                            .ToListAsync();

                        foreach (var demande in demandesModifiees)
                        {
                            var compte = await context.Comptes
                                .Include(c => c.Client)
                                .FirstOrDefaultAsync(c => c.RIB == demande.RIBCompte);

                            if (compte == null || compte.Client == null)
                            {
                                _logger.LogError("Compte ou client introuvable pour le RIB {RIB}", demande.RIBCompte);
                                continue;
                            }

                            string emailSubject;
                            string emailContent;
                            string salutation = compte.Client.Genre == "Féminin" ? "Madame" : "Monsieur";
                            string nomComplet = $"{compte.Client.Prenom} {compte.Client.Nom}";


                            if (demande.StatutDemande == StatutDemandeEnum.Accepte)
                            {
                                // Mise à jour du découvert
                                compte.DecouvertAutorise = demande.DecouvertDemande;
                                // <p>Nous vous rappelons que les agios sont calculés au taux de {GetTauxAgios(compte.Type)}% et sont applicables sur toute utilisation du découvert.</p>
                                emailSubject = "Confirmation d'acceptation de votre demande d'augmentation de découvert";
                                emailContent = $@"
                                <p>{salutation} {nomComplet},</p>                                    
                                    <p>Nous avons le plaisir de vous informer que votre demande d'augmentation de découvert sur votre compte {compte.Type} se terminant par {compte.RIB.Substring(compte.RIB.Length - 4)} a été <strong>acceptée</strong>.</p>
                                    
                                    <p>Votre nouveau plafond de découvert autorisé est désormais de : <strong>{demande.DecouvertDemande} TND</strong>.</p>
                                    
                                    <p>Cette modification est effective immédiatement et sera visible sur votre prochaine édition de relevé de compte.</p>
                                    
                                   
                                    
                                    <p>Pour toute question relative à cette autorisation, votre conseiller dédié reste à votre disposition.</p>
                                    
                                    <p>Cordialement,<br>
                                    Le Service Client<br>
                                    Société Tunisienne de Banque<br>
                                    <small>Ceci est un message automatique, merci de ne pas y répondre</small></p>";
                            }
                            else if (demande.StatutDemande == StatutDemandeEnum.Refuse)
                            {
                                emailSubject = "Notification concernant votre demande de découvert";
                                emailContent = $@"
                                    <p>Madame, Monsieur,</p>
                                    
                                    <p>Suite à l'étude de votre demande d'augmentation de découvert sur votre compte {compte.Type} 010************* {compte.RIB.Substring(compte.RIB.Length - 4)}, nous regrettons de vous informer que nous ne sommes pas en mesure d'y donner suite.</p>
                                    
                                    <p><strong>Motif du refus</strong> :<br>
                                    {demande.MotifRefus}</p>
                                    
                                    <p>Votre découvert actuel reste inchangé à : <strong>{compte.DecouvertAutorise} TND</strong>.</p>
                                    
                                    <p>Pour toute information complémentaire ou pour discuter des alternatives possibles, nous vous invitons à prendre contact avec votre conseiller bancaire.</p>
                                    
                                    <p>Nous vous remercions de votre confiance et restons à votre disposition pour tout accompagnement supplémentaire.</p>
                                    
                                    <p>Cordialement,<br>
                                    Le Service Engagements<br>
                                    Société Tunisienne de Banque<br>
                                    <small>Ceci est un message automatique, merci de ne pas y répondre</small></p>";
                            }
                            else
                            {
                                _logger.LogWarning("Statut inconnu: {Statut}", demande.StatutDemande);
                                continue;
                            }

                            // Envoi du mail HTML
                            await emailService.SendEmailAsync(compte.Client.Email, emailSubject, emailContent);

                            // Mise à jour du statut
                            demande.MailEnvoyee = true;
                            await context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du traitement des demandes");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

       

       
    }
}
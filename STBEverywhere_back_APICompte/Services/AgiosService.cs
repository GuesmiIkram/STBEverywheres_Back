using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace STBEverywhere_back_APICompte.Services
{
    public class AgiosService
    {



        private readonly ApplicationDbContext _context;
        private readonly ILogger<AgiosService> _logger;

        public AgiosService(ApplicationDbContext context, ILogger<AgiosService> logger)
        {
            _context = context;
            _logger = logger;
        }




        public async Task CalculerEtAppliquerAgiosMensuels()
        {
            const decimal TAUX_AGIOS_ANNUEL = 0.12m; // 12% fixe pour tous les comptes

            _logger.LogInformation("Début du calcul mensuel des agios");

            var dateCalcul = DateTime.Now;
            var dateDebutMois = new DateTime(dateCalcul.Year, dateCalcul.Month, 1);
            var dateFinMois = dateDebutMois.AddMonths(1).AddDays(-1);

            try
            {
                var comptes = await _context.Comptes
                    .Include(c => c.PeriodesDecouvert)
                    .Where(c => c.PeriodesDecouvert.Any(p =>
                        (p.DateFin == null || p.DateFin >= dateDebutMois) &&
                        p.DateDebut <= dateFinMois))
                    .ToListAsync();

                foreach (var compte in comptes)
                {
                    decimal totalAgios = 0;
                    //une collection des périodes de découvert pour chaque compte
                    var periodes = compte.PeriodesDecouvert
                        .Where(p => p.DateDebut <= dateFinMois &&
                                   (p.DateFin == null || p.DateFin >= dateDebutMois))
                        .ToList();

                    foreach (var periode in periodes)
                    {
                        var debut = periode.DateDebut < dateDebutMois ? dateDebutMois : periode.DateDebut;
                        var fin = (periode.DateFin ?? dateCalcul) > dateFinMois ? dateFinMois : (periode.DateFin ?? dateCalcul);
                        //.Days pour donner le nombre de jours entiers dans cette durée.
                        var jours = (fin - debut).Days;

                        if (jours > 0)
                        {
                            totalAgios += AgiosCalculator.CalculerAgios(
                                periode.MontantMaxDecouvert,
                                TAUX_AGIOS_ANNUEL, // Taux fixe ici
                                jours);
                        }
                    }

                    if (totalAgios > 0)
                    {
                        compte.Solde -= totalAgios;
                        _context.FraisComptes.Add(new FraisCompte
                        {
                            type = "AgiosDecouvert",
                            Date = dateCalcul,
                            Montant = totalAgios,
                            RIB = compte.RIB,
                           
                        });

                        _logger.LogInformation($"Agios de {totalAgios} appliqués au compte {compte.RIB}");
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Calcul des agios terminé avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du calcul des agios");
                throw;
            }
        }


    }
    }


using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APICompte.Services
{
    public class DecouvertTrackerService
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<DecouvertTrackerService> _logger;

        public DecouvertTrackerService(ApplicationDbContext context, ILogger<DecouvertTrackerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task TrackDecouvert(string rib, decimal ancienSolde, decimal nouveauSolde)
        {
            try
            {
                var periodeActive = await _context.PeriodeDecouverts
                    .FirstOrDefaultAsync(p => p.RIB == rib && p.DateFin == null);

                if (nouveauSolde < 0)
                {
                    if (periodeActive == null)
                    {
                        _context.PeriodeDecouverts.Add(new PeriodeDecouvert
                        {
                            RIB = rib,
                            DateDebut = DateTime.Now,
                            MontantMaxDecouvert = Math.Abs(nouveauSolde),
                            SoldeInitial = ancienSolde
                        });
                        _logger.LogInformation($"Nouvelle période de découvert pour le compte {rib}");
                    }
                    else if (Math.Abs(nouveauSolde) > periodeActive.MontantMaxDecouvert)
                    {
                        periodeActive.MontantMaxDecouvert = Math.Abs(nouveauSolde);
                        _logger.LogInformation($"Mise à jour du découvert max pour le compte {rib}");

                    }

                    else if (periodeActive != null)
                    {
                        periodeActive.DateFin = DateTime.Now;
                        _logger.LogInformation($"Clôture période découvert pour le compte {rib}");
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du suivi du découvert pour le compte {rib}");
                throw;
            }

        }
    }
} 
    

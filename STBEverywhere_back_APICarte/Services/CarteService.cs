using STBEverywhere_back_APICarte.Repository;
using STBEverywhere_Back_SharedModels.Models.DTO;
using STBEverywhere_Back_SharedModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;

namespace STBEverywhere_back_APICarte.Services
{
    public class CarteService : ICarteService
    {
        private readonly ICarteRepository _carteRepository;
        private readonly ILogger<CarteService> _logger;
        private readonly EmailService _emailService;
        private readonly ApplicationDbContext _dbContext;

        public CarteService(ICarteRepository carteRepository, ILogger<CarteService> logger, EmailService emailService, ApplicationDbContext dbContext)
        {
            _carteRepository = carteRepository;
            _logger = logger;
            _emailService = emailService;
            _dbContext = dbContext;

        }

        public async Task<IEnumerable<CarteDTO>> GetCartesByRIBAsync(string rib)
        {
            _logger.LogInformation("Récupération des cartes pour le RIB : {RIB}", rib);
            var cartes = await _carteRepository.GetCartesByRIBAsync(rib);
            return cartes.Select(c => new CarteDTO
            {
                NumCarte = c.NumCarte,
                NomCarte = c.NomCarte,
                TypeCarte = c.TypeCarte,
                DateCreation = c.DateCreation,
                DateExpiration = c.DateExpiration,
                Statut = c.Statut,
                RIB = c.RIB
            });
        }

        public async Task<bool> CreateDemandeCarteAsync(DemandeCarteDTO demandeCarteDTO)
        {
            _logger.LogInformation("Création d'une nouvelle demande de carte pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);

            var cartes = await _carteRepository.GetCartesByRIBAsync(demandeCarteDTO.NumCompte);

            // Condition 1: Maximum 2 cartes internationales par compte
            if (demandeCarteDTO.TypeCarte == "International" && cartes.Count(c => c.TypeCarte == "International") >= 2)
            {
                _logger.LogWarning("Tentative de création d'une troisième carte internationale pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                throw new InvalidOperationException("Un compte ne peut avoir que 2 cartes internationales.");
            }

            // Condition 2: Maximum 2 cartes nationales par compte
            if (demandeCarteDTO.TypeCarte == "National" && cartes.Count(c => c.TypeCarte == "National") >= 2)
            {
                _logger.LogWarning("Tentative de création d'une troisième carte nationale pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                throw new InvalidOperationException("Un compte ne peut avoir que 2 cartes nationales.");
            }

            // Condition 3: Une seule carte épargne par compte
            if (demandeCarteDTO.NomCarte == "Epargne" && cartes.Any(c => c.NomCarte == "Epargne"))
            {
                _logger.LogWarning("Tentative de création d'une deuxième carte épargne pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                throw new InvalidOperationException("Un compte ne peut avoir qu'une seule carte épargne.");
            }

            // Condition 4: Pas de demande en cours avec le même nom et type de carte
            var demandesExistantes = await _carteRepository.GetDemandesByCompteAndNomAndTypeAsync(
                demandeCarteDTO.NumCompte,
                demandeCarteDTO.NomCarte,
                demandeCarteDTO.TypeCarte
            );

            if (demandesExistantes.Any(d => d.Statut != "Recuperee"))
            {
                _logger.LogWarning("Une demande existe déjà avec le même nom et type de carte pour le compte : {NumCompte}", demandeCarteDTO.NumCompte);
                throw new InvalidOperationException("Une demande est déjà en cours avec le même nom et type de carte.");
            }

            // Créer la demande de carte avec le statut "En cours de préparation"
            var demandeCarte = new DemandeCarte
            {
                NumCompte = demandeCarteDTO.NumCompte,
                NomCarte = demandeCarteDTO.NomCarte,
                TypeCarte = demandeCarteDTO.TypeCarte,
                CIN = demandeCarteDTO.CIN,
                Email = demandeCarteDTO.Email,
                NumTel = demandeCarteDTO.NumTel,
                DateCreation = DateTime.Now,
                Statut = "En cours de préparation" // Statut initial
            };

            // Enregistrer la demande de carte
            return await _carteRepository.CreateDemandeCarteAsync(demandeCarte);
        }

        public async Task<bool> CreateCarteIfDemandeRecupereeAsync(int demandeId)
        {
            _logger.LogInformation("Tentative de création de carte pour la demande : {DemandeId}", demandeId);

            // Récupérer la demande de carte
            var demande = await _carteRepository.GetDemandeCarteByIdAsync(demandeId);

            if (demande == null)
            {
                _logger.LogWarning("Demande de carte introuvable : {DemandeId}", demandeId);
                throw new InvalidOperationException("Demande de carte introuvable.");
            }

            // Vérifier si le statut de la demande est "Récupérée"
            if (demande.Statut != "Recuperee")
            {
                _logger.LogWarning("La carte n'est pas encore récupérée : {DemandeId}", demandeId);
                throw new InvalidOperationException("La carte ne peut être créée que si la demande est récupérée.");
            }

            _logger.LogInformation("Création de la carte pour la demande : {DemandeId}", demandeId);
            var codePIN = await GenerateUniquePinAsync();
            var codeCVV = await GenerateUniqueCvvAsync();
            var encryptedPIN = EncryptCode(int.Parse(codePIN));
            var encryptedCVV = EncryptCode(int.Parse(codeCVV));

            // Créer la carte
            var carte = new Carte
            {
                NumCarte = await GenerateUniqueCardNumberAsync(), // Générer un numéro de carte unique
                NomCarte = demande.NomCarte,
                TypeCarte = demande.TypeCarte,
                DateCreation = demande.DateCreation,
                DateExpiration = demande.DateCreation.AddYears(3),
                Statut = "Active",
                RIB = demande.NumCompte,
                Plafond = 1000,
                Iddemande = demande.Iddemande,
                DateRecuperation = DateTime.Now,
                CodePIN = encryptedPIN,
                CodeCVV = encryptedCVV
            };

            // Enregistrer la carte dans la base de données
            var result = await _carteRepository.CreateCarteAsync(carte);

            if (result)
            {
                _logger.LogInformation("Carte créée avec succès pour la demande : {DemandeId}", demandeId);
            }
            else
            {
                _logger.LogError("Échec de la création de la carte pour la demande : {DemandeId}", demandeId);
            }

            return result;
        }

        private string EncryptCode(int code)
        {
            // Convertir le code en chaîne de caractères
            string codeString = code.ToString();

            // Crypter le code avec BCrypt
            return BCrypt.Net.BCrypt.HashPassword(codeString);
        }

        public async Task<IEnumerable<DemandeCarte>> GetDemandesByStatutAsync(string statut)
        {
            _logger.LogInformation("Récupération des demandes avec le statut : {Statut}", statut);

            // Récupérer les demandes avec le statut spécifié
            var demandes = await _carteRepository.GetDemandesByStatutAsync(statut);

            _logger.LogInformation("Nombre de demandes récupérées : {Count}", demandes.Count());

            // Log des détails des demandes récupérées
            foreach (var demande in demandes)
            {
                _logger.LogInformation("Détails de la demande : Id={Iddemande}, NumCompte={NumCompte}, NomCarte={NomCarte}, TypeCarte={TypeCarte}, Statut={Statut}",
                    demande.Iddemande, demande.NumCompte, demande.NomCarte, demande.TypeCarte, demande.Statut);
            }

            return demandes;
        }

        private async Task<string> GenerateUniqueCardNumberAsync()
        {
            string cardNumber;
            bool isUnique;

            do
            {
                // Générer un numéro de carte aléatoire de 16 chiffres
                cardNumber = GenerateRandomCardNumber();

                // Vérifier si le numéro de carte existe déjà dans la base de données
                isUnique = !await _carteRepository.CardNumberExistsAsync(cardNumber);
            } while (!isUnique); // Répéter jusqu'à ce qu'un numéro unique soit généré

            return cardNumber;
        }

        private string GenerateRandomCardNumber()
        {
            var random = new Random();
            var cardNumberBuilder = new StringBuilder();

            // Générer 16 chiffres aléatoires
            for (int i = 0; i < 16; i++)
            {
                cardNumberBuilder.Append(random.Next(0, 10)); // Chiffre entre 0 et 9
            }

            return cardNumberBuilder.ToString();
        }

        private async Task<string> GenerateUniquePinAsync()
        {
            string pin;
            bool isUnique;

            do
            {
                pin = GenerateRandomCode(4); // Générer un code PIN à 4 chiffres sous forme de string
                isUnique = !await _carteRepository.PinExistsAsync(pin); // Vérifier l'unicité
            } while (!isUnique);

            return pin;
        }

        private async Task<string> GenerateUniqueCvvAsync()
        {
            string cvv;
            bool isUnique;

            do
            {
                cvv = GenerateRandomCode(3); // Générer un code CVV à 3 chiffres sous forme de string
                isUnique = !await _carteRepository.CvvExistsAsync(cvv); // Vérifier l'unicité
            } while (!isUnique);

            return cvv;
        }

        // Modifier GenerateRandomCode pour retourner une string
        private string GenerateRandomCode(int length)
        {
            Random random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => (char)('0' + random.Next(0, 10)))
                .ToArray());
        }


        public async Task<IEnumerable<DemandeCarteDTO>> GetDemandesByRIBAsync(string rib)
        {
            _logger.LogInformation("Récupération des demandes de carte pour le RIB : {RIB}", rib);
            var demandes = await _carteRepository.GetDemandesByRIBAsync(rib);
            return demandes.Select(d => new DemandeCarteDTO
            {
                Iddemande = d.Iddemande,
                NumCompte = d.NumCompte,
                NomCarte = d.NomCarte,
                TypeCarte = d.TypeCarte,
                CIN = d.CIN,
                Email = d.Email,
                NumTel = d.NumTel,
                DateCreation = d.DateCreation,
                Statut = d.Statut,
                ClientId = d.ClientId
            });
        }

        public async Task<CarteDetails> GetCarteDetailsAsync(string numCarte)
        {
            _logger.LogInformation("Récupération des détails de la carte : {NumCarte}", numCarte);

            // Récupérer la carte depuis le repository
            var carte = await _carteRepository.GetCarteByNumCarteAsync(numCarte);

            if (carte == null)
            {
                _logger.LogWarning("Carte introuvable : {NumCarte}", numCarte);
                throw new InvalidOperationException("Carte introuvable.");
            }

            // Retourner les détails de la carte sous forme de CarteDetails
            return new CarteDetails
            {
                Statut = carte.Statut,  // Statut de la carte
                Plafond = carte.Plafond // Plafond de la carte
            };
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                await _emailService.SendEmailAsync(email, subject, message);
                _logger.LogInformation("Email envoyé à {Email} avec le sujet {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi de l'email à {Email}", email);
                throw;
            }
        }
        public async Task<bool> UpdateEmailEnvoyeAsync(int demandeId, bool emailEnvoye)
        {
            return await _carteRepository.UpdateEmailEnvoyeAsync(demandeId, emailEnvoye);
        }

        public async Task<bool> UpdateEmailEnvoyeLivreeAsync(int demandeId, bool emailEnvoyeLivree)
        {
            return await _carteRepository.UpdateEmailEnvoyeLivreeAsync(demandeId, emailEnvoyeLivree);
        }

        public async Task UpdateDemandeAsync(DemandeCarte demande)
        {
            try
            {
                // Vérifier si la demande existe dans la base de données
                var existingDemande = await _dbContext.DemandesCarte
                    .FirstOrDefaultAsync(d => d.Iddemande == demande.Iddemande);

                if (existingDemande == null)
                {
                    throw new InvalidOperationException($"La demande avec l'ID {demande.Iddemande} n'existe pas.");
                }

                // Mettre à jour les propriétés de la demande existante
                _dbContext.Entry(existingDemande).CurrentValues.SetValues(demande);

                // Sauvegarder les modifications dans la base de données
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log l'erreur ou relancer l'exception
                throw new ApplicationException("Une erreur s'est produite lors de la mise à jour de la demande.", ex);
            }
        }
    }
}
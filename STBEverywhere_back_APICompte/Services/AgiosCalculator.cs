namespace STBEverywhere_back_APICompte.Services
{
    public class AgiosCalculator
    {
        public static decimal CalculerAgios(decimal montantDecouvert, decimal tauxAnnuel, int jours)
        {
            if (jours <= 0 || montantDecouvert <= 0)
                return 0;

            decimal tauxJournalier = tauxAnnuel / 365;
            return montantDecouvert * tauxJournalier * jours;
        }
    }
}

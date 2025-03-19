namespace STBEverywhere_Back_SharedModels.Models.enums
{
    public enum StatutDemande
    {
        EnCours,       // La demande est en cours de traitement
        DisponibleEnAgence,      // La demande a été approuvée
        Rejetee,        // La demande a été rejetée
        Recuperee,      // La carte a été récupérée par le client
        Livree,         // La carte a été livrée au client
        EnPreparation   // La carte est en cours de préparation
    }
}

namespace STBEverywhere_back_APIAgent.Service.IService
{
    public interface IReclamationService
    {
        Task<bool> RepondreAReclamationAsync(int reclamationId, string contenuReponse, int idAgent);
    }

}

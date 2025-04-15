using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIAgent.Repository.IRepository
{
    public interface IReclamationRepository
    {
        Task<Reclamation> GetByIdAsync(int id);
        Task UpdateAsync(Reclamation reclamation);
    }

}

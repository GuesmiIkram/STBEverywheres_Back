using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIChequier.Repository.IRepositoy
{
    public interface IEmailLogRepository
    {
        Task<EmailLog> GetExistingEmailLogAsync(int demandeId, string emailType);
        Task AddEmailLogAsync(EmailLog emailLog);
        Task SaveChangesAsync();
    }
}

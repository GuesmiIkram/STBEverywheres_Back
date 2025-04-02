using Microsoft.EntityFrameworkCore;
using STBEverywhere_back_APICompte.Repository.IRepository;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APICompte.Repository
{
    public class EmailLogRepository : IEmailLogRepository
    {

        private readonly ApplicationDbContext _context;

        public EmailLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmailLog> GetExistingEmailLogAsync(int demandeId, string emailType)
        {
            return await _context.EmailLogs
                .Where(e => e.DemandeId == demandeId && e.IsEnvoye && e.EmailType == emailType)
                .FirstOrDefaultAsync();
        }

        public async Task AddEmailLogAsync(EmailLog emailLog)
        {

            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

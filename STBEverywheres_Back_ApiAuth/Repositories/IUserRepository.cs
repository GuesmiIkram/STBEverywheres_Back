using System.Threading.Tasks;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_ApiAuth.Repositories
{


    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id, bool includeRelated = false);
        Task<User> GetByEmailAsync(string email, bool includeClient = false);
        Task<User> GetUserWithClientByEmailAsync(string email);
        Task AddAsync(User user);
        bool VerifyPassword(User user, string password);
        void UpdatePassword(User user, string newPassword);
        Task UpdateAsync(User user);
        Task DeleteAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task UpdatePasswordAsync(int userId, string newPasswordHash);
        Task<User> GetByResetTokenAsync(string resetToken);
      Task<Client?> GetClientByUserIdAsync(int userId);
        Task<Agent?> GetAgentByUserIdAsync(int userId); 

    }
}
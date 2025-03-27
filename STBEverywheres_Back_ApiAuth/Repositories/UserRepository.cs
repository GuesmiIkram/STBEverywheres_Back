using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using STBEverywhere_Back_SharedModels;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_Back_SharedModels.Models;
using System;
using System.Threading.Tasks;

namespace STBEverywhere_ApiAuth.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> GetByIdAsync(int id, bool includeRelated = false)
        {
            _logger.LogDebug("Fetching user with ID: {UserId}", id);

            try
            {
                var query = _context.Users.AsQueryable();

             

                return await query
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID: {UserId}", id);
                throw;
            }
        }

        public async Task<User> GetByEmailAsync(string email, bool includeClient = false)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Empty email provided");
                throw new ArgumentException("Email cannot be empty", nameof(email));
            }

            _logger.LogDebug("Fetching user with email: {Email}", email);

            try
            {
                var query = _context.Users.AsQueryable();

               
                return await query
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with email: {Email}", email);
                throw;
            }
        }

        // Modifiez cette méthode
        public async Task<User> GetUserWithClientByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Empty email provided");
                throw new ArgumentException("Email cannot be empty", nameof(email));
            }

            _logger.LogDebug("Fetching user with client by email: {Email}", email);

            try
            {
                // Utilisez votre méthode existante GetClientByUserIdAsync
                var user = await GetByEmailAsync(email);
                if (user == null) return null;

                var client = await GetClientByUserIdAsync(user.Id);
                // Vous pouvez attacher le client à l'utilisateur si nécessaire
                // Mais comme vous ne voulez pas de propriété de navigation, vous pourriez
                // retourner un DTO ou un objet anonyme à la place
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with client by email: {Email}", email);
                throw;
            }
        }
        public async Task AddAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempt to add null user");
                throw new ArgumentNullException(nameof(user));
            }

            if (await EmailExistsAsync(user.Email))
            {
                _logger.LogWarning("Duplicate email: {Email}", user.Email);
                throw new InvalidOperationException($"Email {user.Email} already exists");
            }

            _logger.LogInformation("Adding new user with email: {Email}", user.Email);

            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                _context.Entry(user).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user with email: {Email}", user.Email);
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempt to update null user");
                throw new ArgumentNullException(nameof(user));
            }

            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _context.Entry(user).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", user.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int userId)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", userId);

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    return;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                return await _context.Users
                    .AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Email}", email);
                throw;
            }
        }

        public async Task UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            _logger.LogInformation("Updating password for user ID: {UserId}", userId);

            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    throw new KeyNotFoundException($"User with ID {userId} not found");
                }

                user.PasswordHash = newPasswordHash;
                await _context.SaveChangesAsync();
                _context.Entry(user).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password for user ID: {UserId}", userId);
                throw;
            }
        }
        public async Task<User> GetByResetTokenAsync(string resetToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.ResetPasswordToken == resetToken);
        }
        public async Task<Client?> GetClientByUserIdAsync(int userId)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

    }
}

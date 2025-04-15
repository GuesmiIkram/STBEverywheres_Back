using STBEverywhere_Back_SharedModels.Data;
using Microsoft.EntityFrameworkCore;

namespace STBEverywhere_back_APIClient.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task NotifyPackStatusChange(int clientId, string packType, int packId, string newStatus)
        {
            try
            {
                var title = $"Mise à jour de votre pack {packType}";
                var message = $"Le statut de votre pack {packType} a changé: {newStatus}";

                var notification = new NotificationPack
                {
                    ClientId = clientId,
                    Title = title,
                    Message = message,
                    IsRead = false,
                    NotificationType = "PackStatusChange",
                    RelatedPackId = packId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.NotificationsPack.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la notification");
                throw;
            }
        }

        public async Task<IEnumerable<NotificationPack>> GetClientNotifications(int clientId)
        {
            return await _context.NotificationsPack
                .Where(n => n.ClientId == clientId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsRead(int notificationId)
        {
            var notification = await _context.NotificationsPack.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}

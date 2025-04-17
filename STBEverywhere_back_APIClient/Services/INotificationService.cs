using STBEverywhere_Back_SharedModels.Models;

namespace STBEverywhere_back_APIClient.Services
{
    public interface INotificationService
    {
        Task NotifyPackStatusChange(int clientId, string packType, int packId, string newStatus);
        Task<IEnumerable<NotificationPack>> GetClientNotifications(int clientId);
        Task MarkAsRead(int notificationId);
        Task NotifyReclamationStatusChange(int clientId, int reclamationId, string newStatus);
        Task<IEnumerable<NotificationReclamation>> GetClientReclamationNotifications(int clientId);
        Task MarkReclamationNotificationAsRead(int notificationId);
    }


}

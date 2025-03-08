using Microsoft.AspNetCore.SignalR;

namespace STBEverywhere_back_APIChequier.Hubs
{
    public class NotificationHub:Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}

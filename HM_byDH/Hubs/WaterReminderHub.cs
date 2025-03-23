using Microsoft.AspNetCore.SignalR;

namespace HM_byDH.Hubs
{
    public class WaterReminderHub : Hub
    {
        public async Task SendWaterReminder(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveWaterReminder", message);
        }
    }
}

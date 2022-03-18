using TheBugTrucker.Models;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class NotificationService : INotificationService
    {
        public Task AddNotificationAsync(Notification notification)
        {
            throw new NotImplementedException();
        }

        public Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            throw new NotImplementedException();
        }

        public Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            throw new NotImplementedException();
        }
    }
}

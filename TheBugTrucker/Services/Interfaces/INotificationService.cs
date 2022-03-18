using TheBugTrucker.Models;

namespace TheBugTrucker.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetSentNotificationsAsync(string userId);
        Task<List<Notification>> GetReceivedNotificationsAsync(string userId);
        Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);
        Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members);
        Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);
    }
}
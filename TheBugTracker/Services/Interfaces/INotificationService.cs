using TheBugTracker.Models;

namespace TheBugTracker.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(Notification notification);
        Task<Notification> GetNotificationByIdAsync(int? id);
        Task<List<Notification>> GetSentNotificationsAsync(string userId);
        Task<List<Notification>> GetReceivedNotificationsAsync(string userId);
        Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);
        Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members);
        Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);
        Task<Notification> CreateNotification(Ticket ticket);
        void SoftDelete(Notification notification, BTUser user);
        void HardDelete(Notification notification);
    }
}
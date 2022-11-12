namespace TheBugTracker.Models.ViewModels;

public class DisplayNotificationsViewModel
{
    public List<Notification> SentNotifications { get; set; } = new();
    public List<Notification> ReceivedNotifications { get; set; } = new();
}
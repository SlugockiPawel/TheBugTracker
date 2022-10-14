#nullable enable
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Models.Enums;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public sealed class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IRolesService _rolesService;
        private readonly IProjectService _projectService;

        public NotificationService(
            ApplicationDbContext context,
            IEmailSender emailSender,
            IRolesService rolesService,
            IProjectService projectService
        )
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Notification> GetNotificationByIdAsync(int? id)
        {
            try
            {
                return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                return await _context.Notifications
                    .Include(n => n.Recipient)
                    .Include(n => n.Sender)
                    .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                    .Where(n => n.SenderId == userId && !n.DeletedBySender)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                return await _context.Notifications
                    .Include(n => n.Recipient)
                    .Include(n => n.Sender)
                    .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                    .Where(n => n.RecipientId == userId && !n.DeletedByRecipient)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task SendEmailNotificationsByRoleAsync(
            Notification notification,
            int companyId,
            string role
        )
        {
            try
            {
                List<BTUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach (BTUser member in members)
                {
                    notification.RecipientId = member.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task SendMembersEmailNotificationsAsync(
            Notification notification,
            List<BTUser> members
        )
        {
            try
            {
                foreach (BTUser member in members)
                {
                    notification.RecipientId = member.Id;
                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(
            Notification notification,
            string emailSubject
        )
        {
            BTUser? user = await _context.Users.FirstOrDefaultAsync(
                u => u.Id == notification.RecipientId
            );

            if (user is null)
                return false;

            try
            {
                await _emailSender.SendEmailAsync(user.Email, emailSubject, notification.Message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public Notification CreateNotification(Ticket? ticket, string title, string message, BTUser sender, BTUser recipient)
        {

            Notification notification =
                new()
                {
                    Created = DateTimeOffset.UtcNow,
                    Message = message,
                    Sender = sender,
                    Ticket = ticket,
                    Title = title,
                    SenderId = sender.Id,
                    Viewed = false,
                    TicketId = ticket?.Id,
                    Recipient = recipient,
                    RecipientId = recipient.Id
                };

            

            return notification;
        }
        

        public void SoftDelete(Notification notification, BTUser user)
        {
            try
            {
                if (notification.SenderId == user.Id)
                {
                    notification.DeletedBySender = true;
                }

                if (notification.RecipientId == user.Id)
                {
                    notification.DeletedByRecipient = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void HardDelete(Notification notification)
        {
            try
            {
                _context.Notifications.Remove(notification);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TheBugTrucker.Data;
using TheBugTrucker.Models;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IRolesService _rolesService;

        public NotificationService(ApplicationDbContext context, IEmailSender emailSender, IRolesService rolesService)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
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

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                return await _context.Notifications
                    .Include(n => n.Recipient)
                    .Include(n => n.Sender)
                    .Include(n => n.Ticket)
                    .ThenInclude(t => t.Project)
                    .Where(n => n.SenderId == userId)
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
                    .Where(n => n.RecipientId == userId)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
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

        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
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

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            BTUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

            if (user is null) return false;

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
    }
}
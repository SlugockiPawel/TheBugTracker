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

        public async Task<Notification> CreateNotification(Ticket ticket)
        {
            var projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);

            Notification notification = new()
            {
                Created = DateTimeOffset.UtcNow,
                Message = "New ticket created",
                Sender = ticket.OwnerUser,
                Ticket = ticket,
                Title = ticket.Title,
                SenderId = ticket.OwnerUserId,
                Viewed = false,
                TicketId = ticket.Id
            };

            if (projectManager is not null)
            {
                notification.Recipient = projectManager;
                notification.RecipientId = projectManager.Id;
            }
            else
            {
                var admin = (
                    await _projectService.GetProjectMembersByRoleAsync(
                        ticket.ProjectId,
                        nameof(Roles.Admin)
                    )
                ).FirstOrDefault();

                if (admin is not null)
                {
                    notification.Recipient = admin;
                    notification.RecipientId = admin.Id;
                }
            }

            return notification;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public class TicketHistoryService : ITicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public TicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // TODO try iterate through TicketHistory properties and write method that checks each property if it has been changed

        /// <summary>
        /// Adds history (TicketHistory object) to the specified Ticket object. oldTicket and newTicket are the same Ticket object (newTicket will update oldTicket in the database)
        /// </summary>
        /// <param name="oldTicket">Current Ticket in the database</param>
        /// <param name="newTicket">Potential future Ticket that will replace oldTicket in the database (if any changes made to the Ticket)</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            // New Ticket has been added
            if (oldTicket is null && newTicket is not null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Ticket Created",
                };

                try
                {
                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            else // this is an existing ticket that could have been changed
            {
                // Ticket title change
                if (oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket title: {newTicket.Title}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Ticket description change
                if (oldTicket.Description != newTicket.Description)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket description: {newTicket.Description}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Ticket priority change
                if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketPriority",
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = newTicket.TicketPriority.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket priority: {newTicket.TicketPriority.Name}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Ticket status change
                if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketStatus",
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = newTicket.TicketStatus.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket status: {newTicket.TicketStatus.Name}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Ticket type change
                if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "TicketType",
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = newTicket.TicketType.Name,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket type: {newTicket.TicketType.Name}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Ticket developer change
                if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Developer",
                        OldValue = oldTicket.DeveloperUser?.FullName ?? "Not Assigned",
                        NewValue = newTicket.DeveloperUser?.FullName,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket developer: {newTicket.DeveloperUser.FullName}",
                    };

                    await _context.TicketHistories.AddAsync(history);
                }

                // Save the TicketHistory DbSet to the database
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects
                    .Where(p => p.CompanyId == companyId)
                    .Include(p => p.Tickets)
                    .ThenInclude(t => t.History)
                    .ThenInclude(h => h.User)
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                return project.Tickets.SelectMany(t => t.History).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                // return await _context.TicketHistories
                //     .Where(th => th.Ticket.Project.CompanyId == companyId)
                //     .Include(th => th.User)
                //     .Include(th => th.Ticket.History)
                //     .Include(th => th.Ticket)
                //     .Include(th => th.Ticket.Project)
                //     .ToListAsync();

                //TODO compare the below and above implementations and see if we have same results - one is going down from Companies, another is going up from TicketHistory

                List<Project> projects = (await _context.Companies
                    .Include(c => c.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.History)
                    .ThenInclude(h => h.User)
                    .FirstOrDefaultAsync(c => c.Id == companyId))?.Projects.ToList();

                List<Ticket> tickets = projects.SelectMany(p => p.Tickets).ToList();

                List<TicketHistory> ticketHistories = tickets.SelectMany(t => t.History).ToList();

                return ticketHistories;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using TheBugTrucker.Data;
using TheBugTrucker.Models;
using TheBugTrucker.Models.Enums;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRolesService _rolesService;
        private readonly IProjectService _projectService;


        public TicketService(ApplicationDbContext context, IRolesService rolesService, IProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        public async Task AddNewTicketAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            ticket.Archived = true;
            await UpdateTicketAsync(ticket);
        }

        public Task AssignTicketAsync(int ticketId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            try
            {
                return (await GetAllTicketsByCompanyAsync(companyId))
                    .Where(t => t.Archived)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                // TODO the one below is mine, check if it work as desired- if not, the other one below should work
                return await _context.Tickets
                    .Where(t => t.Project.Company.Id == companyId)
                    .Include(t => t.TicketAttachments)
                    .Include(t => t.Comments)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.History)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .ToListAsync();

                // return await _context.Projects
                //     .Where(p => p.CompanyId == companyId)
                //     .SelectMany(p => p.Tickets)
                //     .Include(t => t.TicketAttachments)
                //     .Include(t => t.Comments)
                //     .Include(t => t.DeveloperUser)
                //     .Include(t => t.History)
                //     .Include(t => t.OwnerUser)
                //     .Include(t => t.TicketPriority)
                //     .Include(t => t.TicketStatus)
                //     .Include(t => t.TicketType)
                //     .Include(t => t.Project)
                //     .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            // int? priorityId = await LookupTicketPriorityIdAsync(priorityName);

            try
            {
                // TODO the one below is mine, check if it work as desired- if not, the other one below should work
                return await _context.Tickets
                    .Where(t => t.Project.Company.Id == companyId && t.TicketPriority.Name == priorityName)
                    .Include(t => t.TicketAttachments)
                    .Include(t => t.Comments)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.History)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .ToListAsync();

                // return await _context.Projects
                //     .Where(c => c.CompanyId == companyId)
                //     .SelectMany(p => p.Tickets)
                //     .Include(t => t.TicketAttachments)
                //     .Include(t => t.Comments)
                //     .Include(t => t.DeveloperUser)
                //     .Include(t => t.History)
                //     .Include(t => t.OwnerUser)
                //     .Include(t => t.TicketPriority)
                //     .Include(t => t.TicketStatus)
                //     .Include(t => t.TicketType)
                //     .Include(t => t.Project)
                //     .Where(t => t.TicketPriority.Name == priorityName)
                //     .ToListAsync();

                // return await _context.Projects
                //     .Where(c => c.CompanyId == companyId)
                //     .SelectMany(p => p.Tickets)
                //     .Include(t => t.TicketAttachments)
                //     .Include(t => t.Comments)
                //     .Include(t => t.DeveloperUser)
                //     .Include(t => t.History)
                //     .Include(t => t.OwnerUser)
                //     .Include(t => t.TicketPriority)
                //     .Include(t => t.TicketStatus)
                //     .Include(t => t.TicketType)
                //     .Include(t => t.Project)
                //     .Where(t => t.TicketPriorityId == priorityId)
                //     .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            // int? statusId = await LookupTicketStatusIdAsync(statusName);

            try
            {
                // TODO the one below is mine, check if it work as desired- if not, the other one below should work
                return await _context.Tickets
                    .Where(t => t.Project.Company.Id == companyId && t.TicketStatus.Name == statusName)
                    .Include(t => t.TicketAttachments)
                    .Include(t => t.Comments)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.History)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .ToListAsync();

                // return await _context.Projects
                //     .Where(c => c.CompanyId == companyId)
                //     .SelectMany(p => p.Tickets)
                //     .Include(t => t.TicketAttachments)
                //     .Include(t => t.Comments)
                //     .Include(t => t.DeveloperUser)
                //     .Include(t => t.History)
                //     .Include(t => t.OwnerUser)
                //     .Include(t => t.TicketPriority)
                //     .Include(t => t.TicketStatus)
                //     .Include(t => t.TicketType)
                //     .Include(t => t.Project)
                //     .Where(t => t.TicketStatus.Name == statusName)
                //     .ToListAsync();

                // return await _context.Projects
                //     .Where(c => c.CompanyId == companyId)
                //     .SelectMany(p => p.Tickets)
                //     .Include(t => t.TicketAttachments)
                //     .Include(t => t.Comments)
                //     .Include(t => t.DeveloperUser)
                //     .Include(t => t.History)
                //     .Include(t => t.OwnerUser)
                //     .Include(t => t.TicketPriority)
                //     .Include(t => t.TicketStatus)
                //     .Include(t => t.TicketType)
                //     .Include(t => t.Project)
                //     .Where(t => t.TicketStatusId == statusId)
                //     .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            // int? ticketTypeId = await LookupTicketTypeIdAsync(typeName);

            try
            {
                // TODO the one below is mine, check if it work as desired- if not, the other one below should work
                return await _context.Tickets
                    .Where(t => t.Project.Company.Id == companyId && t.TicketType.Name == typeName)
                    .Include(t => t.TicketAttachments)
                    .Include(t => t.Comments)
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.History)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .ToListAsync();

                // return await _context.Projects
                //     .Where(c => c.CompanyId == companyId)
                //     .SelectMany(p => p.Tickets)
                //     .Include(t => t.TicketAttachments)
                //     .Include(t => t.Comments)
                //     .Include(t => t.DeveloperUser)
                //     .Include(t => t.History)
                //     .Include(t => t.OwnerUser)
                //     .Include(t => t.TicketPriority)
                //     .Include(t => t.TicketStatus)
                //     .Include(t => t.TicketType)
                //     .Include(t => t.Project)
                //     .Where(t => t.TicketType.Name == typeName)
                //     .ToListAsync();

                // return await _context.Projects
                //     .Where(c => c.CompanyId == companyId)
                //     .SelectMany(p => p.Tickets)
                //     .Include(t => t.TicketAttachments)
                //     .Include(t => t.Comments)
                //     .Include(t => t.DeveloperUser)
                //     .Include(t => t.History)
                //     .Include(t => t.OwnerUser)
                //     .Include(t => t.TicketPriority)
                //     .Include(t => t.TicketStatus)
                //     .Include(t => t.TicketType)
                //     .Include(t => t.Project)
                //     .Where(t => t.TicketTypeId == ticketTypeId)
                //     .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId))
                    .FirstOrDefault(t => t.Id == ticketId);

                if (ticket?.DeveloperUserId is not null)
                {
                    return ticket.DeveloperUser;
                }

                return new BTUser();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                if (role == Roles.Admin.ToString())
                {
                    tickets = await GetAllTicketsByCompanyAsync(companyId);
                }
                else if (role == Roles.Developer.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId))
                        .Where(t => t.DeveloperUserId == userId)
                        .ToList();
                }
                else if (role == Roles.Submitter.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId))
                        .Where(t => t.OwnerUserId == userId)
                        .ToList();
                }
                else if (role == Roles.ProjectManager.ToString())
                {
                    tickets = await GetTicketsByUserIdAsync(userId, companyId);
                }

                return tickets;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<Ticket> tickets = new();

            try
            {
                if (await _rolesService.IsUserInRoleAsync(user, Roles.Admin.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                        .SelectMany(p => p.Tickets)
                        .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(user, Roles.Developer.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                        .SelectMany(p => p.Tickets)
                        .Where(t => t.DeveloperUserId == userId)
                        .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(user, Roles.Submitter.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                        .SelectMany(p => p.Tickets)
                        .Where(t => t.OwnerUserId == userId)
                        .ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                {
                    tickets = (await _projectService.GetUserProjectsAsync(userId))
                        .SelectMany(p => p.Tickets)
                        .ToList();
                }

                return tickets;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                return (await _context.TicketPriorities.FirstOrDefaultAsync(tp => tp.Name == priorityName))?.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                return (await _context.TicketStatuses.FirstOrDefaultAsync(ts => ts.Name == statusName))?.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                return (await _context.TicketTypes.FirstOrDefaultAsync(tt => tt.Name == typeName))?.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
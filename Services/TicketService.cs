﻿using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Models.Enums;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public sealed class TicketService : ITicketService
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
            try
            {
                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task AddTicketCommentAsync(TicketComment ticketComment)
        {
            try
            {
                await _context.AddAsync(ticketComment);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.Project)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Comments)
                    .Include(t => t.TicketAttachments)
                    .Include(t => t.History)
                    .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                await UpdateTicketAsync(ticket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            try
            {
                if (ticket is not null)
                {
                    try
                    {
                        ticket.DeveloperUserId = userId;
                        // TODO revisit code below when assigning Tickets
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.Project)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
        {
            try
            {
                return await _context.TicketAttachments
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
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

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId,
            int companyId)
        {
            try
            {
                return (await GetTicketsByRoleAsync(role, userId, companyId))
                    .Where(t => t.ProjectId == projectId)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            try
            {
                return (await GetAllTicketsByStatusAsync(companyId, statusName))
                    .Where(t => t.ProjectId == projectId)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId,
            int projectId)
        {
            try
            {
                return (await GetAllTicketsByPriorityAsync(companyId, priorityName))
                    .Where(t => t.ProjectId == projectId)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            try
            {
                return (await GetAllTicketsByTypeAsync(companyId, typeName))
                    .Where(t => t.ProjectId == projectId)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId)
        {
            try
            {
                return (await GetAllTicketsByCompanyAsync(companyId))
                    .Where(t => string.IsNullOrWhiteSpace(t.DeveloperUserId))
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
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
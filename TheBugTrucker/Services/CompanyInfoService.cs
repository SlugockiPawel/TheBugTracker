using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using TheBugTrucker.Data;
using TheBugTrucker.Models;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class CompanyInfoService : ICompanyInfoService
    {
        private readonly ApplicationDbContext _context;

        public CompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            if (companyId is not null) 
            {
                return (await _context.Companies
                    .Include(c => c.Members)
                    .Include(c => c.Projects)
                    .Include(c => c.Invites)
                    .FirstOrDefaultAsync(c => c.Id == companyId))!;
            }

            return new Company();
        }

        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            return await _context.Users
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<List<Project>> GetAllProjectsAsync(int companyId)
        {
            return await _context.Projects
                .Where(p => p.CompanyId == companyId)
                .Include(p => p.Members)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.Comments)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketStatus)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketPriority)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketType)
                .Include(p => p.ProjectPriority)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketAttachments)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.History)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.DeveloperUser)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.OwnerUser)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.Notifications)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetAllTicketsAsync(int companyId)
        {
            List<Project> projects = await GetAllProjectsAsync(companyId);

            return projects.SelectMany(p => p.Tickets).ToList();
        }
    }
}
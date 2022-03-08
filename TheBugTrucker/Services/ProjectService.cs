using Microsoft.EntityFrameworkCore;
using TheBugTrucker.Data;
using TheBugTrucker.Models;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            await _context.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        public Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
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

        public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            return (await GetAllProjectsByCompanyAsync(companyId))
                .Where(p => p.Archived)
                .ToList();
        }

        public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            return (await _context.Projects
                .Include(p => p.Tickets)
                .Include(p => p.Members)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId))!;
        }

        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserOnProject(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<int> LookupProjectPriorityId(string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using TheBugTrucker.Data;
using TheBugTrucker.Models;
using TheBugTrucker.Models.Enums;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRolesService _rolesService;

        public ProjectService(ApplicationDbContext context, IRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
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

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser user = (await _context.Users.FirstOrDefaultAsync(u => u.Id == userId))!;

            if (user is null) return false;

            Project project = (await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId))!;

            if (!await IsUserOnProjectAsync(userId, projectId))
            {
                try
                {
                    project.Members.Add(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error during adding user to project: {e}, message: {e.Message}");
                    throw;
                }
            }

            return false;
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
                .Where(p => p.CompanyId == companyId && !p.Archived)
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

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            return (await GetAllProjectsByCompanyAsync(companyId))
                .Where(p => p.ProjectPriority.Name == priorityName)
                .ToList();
        }

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            IEnumerable<BTUser> teamMembers = new Collection<BTUser>();

            foreach (string name in Enum.GetNames(typeof(Roles)))
            {
                if (name != Roles.ProjectManager.ToString())
                {
                    List<BTUser> list = await GetProjectMembersByRoleAsync(projectId, name);
                    teamMembers = teamMembers.Concat(list);
                }
            }

            return teamMembers.ToList();
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

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach (BTUser member in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }

            return null;
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = (await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId))!;

            List<BTUser> members = new();

            foreach (BTUser member in project.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(member, role))
                {
                    members.Add(member);
                }
            }

            return members;
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

        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            return await _context.Users
                .Where(u => u.Projects.All(p => p.Id != projectId) && u.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                return (await _context.Users
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                    .ThenInclude(p => p.Tickets)
                    .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u => u.Id == userId))!.Projects.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error getting user projects list: {e}, message: {e.Message}");
                throw;
            }
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            return (await _context.Projects
                    .Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.Id == projectId))!
                .Members.Any(m => m.Id == userId);
        }

        public async Task<int> LookupProjectPriorityIdAsync(string priorityName)
        {
            return (await _context.ProjectPriorities.FirstOrDefaultAsync(pp => pp.Name == priorityName))!.Id;
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = (await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId))!;

                foreach (BTUser user in members)
                {
                    project.Members.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error removing user from the project: {e}, message: {e.Message}");
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            BTUser user = (await _context.Users.FirstOrDefaultAsync(u => u.Id == userId))!;

            if (user is null) return;

            if (await IsUserOnProjectAsync(userId, projectId))
            {
                Project project = (await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId))!;

                try
                {
                    project.Members.Remove(user);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error during removing user from project: {e}, message: {e.Message}");
                    throw;
                }
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
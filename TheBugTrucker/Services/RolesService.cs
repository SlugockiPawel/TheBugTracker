using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public RolesService(RoleManager<IdentityRole> roleManager, ApplicationDbContext context,
            UserManager<BTUser> userManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        #region GetRoles
        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                return await _context.Roles.ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        #endregion

        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            return (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
        }

        public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            return (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            return (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
        }

        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            return (await _userManager.GetUsersInRoleAsync(roleName))
                .Where(u => u.CompanyId == companyId)
                .ToList();
        }

        public async Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName))
                .Select(u => u.Id).ToList();

            return await _context.Users
                .Where(u => !userIds.Contains(u.Id) && u.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole? role = await _context.Roles.FindAsync(roleId);
            return await _roleManager.GetRoleNameAsync(role!);
        }
    }
}
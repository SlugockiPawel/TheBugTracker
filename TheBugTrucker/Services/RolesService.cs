using Microsoft.AspNetCore.Identity;
using TheBugTrucker.Data;
using TheBugTrucker.Models;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public RolesService(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        public Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameByIdAsync(string roleId)
        {
            throw new NotImplementedException();
        }
    }
}

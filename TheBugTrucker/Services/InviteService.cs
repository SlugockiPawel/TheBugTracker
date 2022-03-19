using Microsoft.EntityFrameworkCore;
using TheBugTrucker.Data;
using TheBugTrucker.Models;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Services
{
    public class InviteService : IInviteService
    {
        private readonly ApplicationDbContext _context;

        public InviteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

            if (invite is null) return false;

            try
            {
                // app is accepting this invite, therefore it is no longer in use -> is invalid going forward
                invite.IsValid = false;
                invite.Invitee.Id = userId;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public Task AddNewInviteAsync(Invite invite)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            throw new NotImplementedException();
        }
    }
}
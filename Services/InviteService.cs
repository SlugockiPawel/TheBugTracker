using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public sealed class InviteService : IInviteService
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

        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                await _context.Invites.AddAsync(invite);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                return await _context.Invites
                    .Where(i => i.CompanyId == companyId)
                    .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                return await _context.Invites
                    .Where(i => i.CompanyId == companyId)
                    .Include(i => i.Company)
                    .Include(i => i.Project)
                    .Include(i => i.Invitor)
                    .FirstOrDefaultAsync(i => i.Id == inviteId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                return await _context.Invites
                    .Where(i => i.CompanyId == companyId)
                    .Include(i => i.Company)
                    .Include(i => i.Project)
                    .Include(i => i.Invitor)
                    .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Check if invite is valid based on two conditions:
        /// 1: Invite was issued less than 7 days ago
        /// 2: Invite is still valid (was not used before)
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            if (token is null) return false;

            Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

            if (invite is null) return false;

            return (DateTime.Now - invite.InviteDate.DateTime).TotalDays <= 7
                   &&
                   invite.IsValid;
        }
    }
}
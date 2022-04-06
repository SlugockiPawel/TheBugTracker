using TheBugTracker.Models;

namespace TheBugTracker.Services.Interfaces
{
    public interface ILookupService
    {
        Task<List<TicketPriority>> GetTicketPrioritiesAsync();
        Task<List<TicketStatus>> GetTicketStatusesAsync();
        Task<List<TicketType>> GetTicketTypesAsync();
        Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
    }
}

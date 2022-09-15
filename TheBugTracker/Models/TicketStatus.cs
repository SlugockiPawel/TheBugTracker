using System.ComponentModel;

namespace TheBugTracker.Models
{
    public sealed class TicketStatus
    {
        public int Id { get; set; }

        [DisplayName("Status Name")]
        public string Name { get; set; } = default!;
    }
}

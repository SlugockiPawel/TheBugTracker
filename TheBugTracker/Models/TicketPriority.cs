using System.ComponentModel;

namespace TheBugTracker.Models
{
    public sealed class TicketPriority
    {
        public int Id { get; set; }

        [DisplayName("Priority Name")]
        public string Name { get; set; } = default!;
    }
}

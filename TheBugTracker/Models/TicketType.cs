using System.ComponentModel;

namespace TheBugTracker.Models
{
    public sealed class TicketType
    {
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string Name { get; set; } = default!;
    }
}

using System.ComponentModel;

namespace TheBugTrucker.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }

        [DisplayName("Status Name")]
        public string Name { get; set; } = default!;
    }
}

using System.ComponentModel;

namespace TheBugTrucker.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string Name { get; set; } = default!;
    }
}

using System.ComponentModel;

namespace TheBugTrucker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [DisplayName("Member Comment")] public string Comment { get; set; } = default!;

        [DisplayName("Date")] public DateTimeOffset Created { get; set; }

        [DisplayName("Ticket")] public int TicketId { get; set; }

        [DisplayName("Team Member")] public string UserId { get; set; } = default!;

        // Navigation properties
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; } = default!;
    }
}
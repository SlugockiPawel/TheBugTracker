using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public sealed class TicketComment
    {
        public int Id { get; set; }

        [DisplayName("Member Comment")] public string Comment { get; set; } = default!;

        [DataType(DataType.Date)]
        [DisplayName("Date")] 
        public DateTimeOffset Created { get; set; }

        [DisplayName("Ticket")] public int TicketId { get; set; }

        [DisplayName("Team Member")] public string UserId { get; set; } = default!;

        // Navigation properties
        public Ticket Ticket { get; set; } = default!;
        public BTUser User { get; set; } = default!;
    }
}
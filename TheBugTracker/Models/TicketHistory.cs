using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public sealed class TicketHistory
    {
        public int Id { get; set; }

        [DisplayName("Ticket")] public int TicketId { get; set; } = default!;

        [DisplayName("Updated Item")] public string Property { get; set; } = default!;

        [DisplayName("Previous")] public string OldValue { get; set; } = default!;

        [DisplayName("Current")] public string NewValue { get; set; } = default!;

        [DisplayName("Date Modified")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }
        [DisplayName("Description of Change")] public string Description { get; set; } = default!;

        [DisplayName("Team Member")] public string UserId { get; set; } = default!;


        // Navigation Properties
        public Ticket Ticket { get; set; } = default!;
        public BTUser User { get; set; } = default!;
    }
}
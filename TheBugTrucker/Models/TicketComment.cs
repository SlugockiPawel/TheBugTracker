using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTrucker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [DisplayName("Member Comment")] public string Comment { get; set; } = default!;

        [DataType(DataType.Date)]
        [DisplayName("Date")] 
        public DateTimeOffset Created { get; set; }

        [DisplayName("Ticket")] public int TicketId { get; set; }

        [DisplayName("Team Member")] public string UserId { get; set; } = default!;

        // Navigation properties
        public virtual Ticket Ticket { get; set; } = default!;
        public virtual BTUser User { get; set; } = default!;
    }
}
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public sealed class Notification
    {
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Message { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string RecipientId { get; set; } = default!;

        [Required]
        [DisplayName("Sender")]
        public string SenderId { get; set; } = default!;

        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }

        // NP
        public Ticket Ticket { get; set; } = default!;
        public BTUser Recipient { get; set; } = default!;
        public BTUser Sender { get; set; } = default!;
    }
}

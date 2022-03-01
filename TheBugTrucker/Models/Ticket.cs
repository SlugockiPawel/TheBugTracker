using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTrucker.Models
{
    public class Ticket
    {
        // Primary Key
        public int Id { get; set; }

        [Required] [StringLength(50)] public string Title { get; set; } = default!;

        [Required] public string Description { get; set; } = default!;

        [DataType(DataType.Date)] public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)] public DateTimeOffset? Updated { get; set; }

        public bool Archived { get; set; }

        // Foreign Keys
        [DisplayName("Project")] public int ProjectId { get; set; }

        [DisplayName("Ticket Type")] public int TicketTypeId { get; set; }

        [DisplayName("Ticket Priority")] public int TicketPriorityId { get; set; }

        [DisplayName("Ticket Status")] public int TicketStatusId { get; set; }

        [DisplayName("Ticket Owner")] public string OwnerUserId { get; set; } = default!;

        [DisplayName("Ticket Developer")] public string DeveloperUserId { get; set; } = default!;

        // Navigation Properties
        public virtual Project Project { get; set; } = default!;
        public virtual TicketType TicketType { get; set; } = default!;
        public virtual TicketPriority TicketPriority { get; set; } = default!;
        public virtual TicketStatus TicketStatus { get; set; } = default!;
        public virtual BTUser OwnerUser { get; set; } = default!;
        public virtual BTUser DeveloperUser { get; set; } = default!;

        // one-to-many relationships
        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
    }
}
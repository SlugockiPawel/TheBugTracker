using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;

namespace TheBugTracker.Models
{
    public class Project
    {
        //
        public int Id { get; set; }

        // FK
        [DisplayName("Company")] public int? CompanyId { get; set; }
        [DisplayName("Priority")] public int? ProjectPriorityId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;
        public bool Archived { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        public DateTimeOffset EndDate { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile FormFile { get; set; } = default!;

        [DisplayName("File Name")] public string FileName { get; set; } = default!;
        public byte[] FileData { get; set; } = default!;
        [DisplayName("File Extension")] public string FileContentType { get; set; } = default!;

        // NP
        public virtual Company Company { get; set; } = default!;
        public virtual ProjectPriority ProjectPriority { get; set; } = default!;

        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
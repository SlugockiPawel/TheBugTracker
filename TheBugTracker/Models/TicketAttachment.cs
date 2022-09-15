using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using TheBugTracker.Extensions;

namespace TheBugTracker.Models
{
    public sealed class TicketAttachment
    {
        public int Id { get; set; }

        [DisplayName("Ticket")] public int TicketId { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("File Date")] 
        public DateTimeOffset Created { get; set; }

        [DisplayName("Team Member")] public string UserId { get; set; } = default!;
        [DisplayName("File Description")] public string Description { get; set; } = default!;

        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024*1024)]
        [AllowedExtensions(new string[]{".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf"})]
        public IFormFile FormFile { get; set; } = default!;

        [DisplayName("File Name")] public string FileName { get; set; } = default!;
        public byte[] FileData { get; set; } = default!;

        [DisplayName("File Extension")] public string FileContentType { get; set; } = default!;


        // Navigation Properties
        public Ticket Ticket { get; set; } = default!;
        public BTUser User { get; set; } = default!;
    }
}
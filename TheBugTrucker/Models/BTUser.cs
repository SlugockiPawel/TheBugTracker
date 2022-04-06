using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TheBugTracker.Models
{
    /// <summary>
    /// BTUser == Bug Trucker User
    /// </summary>
    public class BTUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = default!;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = default!;

        [NotMapped][Display(Name = "Full Name")]  public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile AvatarFormFile { get; set; } = default!;

        [DisplayName("Avatar")]
        public string AvatarFormName { get; set; } = default!;
        public byte[] AvatarFileData { get; set; } = default!;

        [DisplayName("File Extension")]
        public string AvatarContentType { get; set; } = default!;

        // FK
        public int CompanyId { get; set; }

        // NP
        public virtual Company Company { get; set; } = default!;

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

    }
}
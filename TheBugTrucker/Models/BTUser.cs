using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TheBugTrucker.Models
{
    public class BTUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = default!;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = default!;

        [NotMapped][Display(Name = "Full Name")]  public string FullName => $"{FirstName} {LastName}";
    }
}
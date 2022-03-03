using System.ComponentModel;

namespace TheBugTrucker.Models
{
    public class Company
    {
        public int Id { get; set; }

        [DisplayName("Company Name")] public string Name { get; set; } = default!;
        [DisplayName("Company Description")] public string Description { get; set; } = default!;

        // NP
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Project> Project { get; set; } = new HashSet<Project>();
    }
}
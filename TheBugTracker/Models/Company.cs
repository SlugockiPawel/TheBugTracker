using System.ComponentModel;

namespace TheBugTracker.Models
{
    public sealed class Company
    {
        public int Id { get; set; }

        [DisplayName("Company Name")] public string Name { get; set; } = default!;
        [DisplayName("Company Description")] public string Description { get; set; } = default!;

        // NP
        public ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
    }
}
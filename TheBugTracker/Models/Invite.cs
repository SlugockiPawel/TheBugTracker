using System.ComponentModel;

namespace TheBugTracker.Models
{
    public class Invite
    {
        // PK
        public int Id { get; set; }

        // FK
        [DisplayName("Company")] public int CompanyId { get; set; }
        [DisplayName("Project")] public int ProjectId { get; set; }
        [DisplayName("Invitor")] public string InvitorId { get; set; } = default!;
        [DisplayName("Invitee")] public string InviteeId { get; set; } = default!;

        [DisplayName("Invitee Email")] public string InviteeEmail { get; set; } = default!;
        [DisplayName("Invitee First Name")] public string InviteeFirstName { get; set; } = default!;
        [DisplayName("Invitee Last Name")] public string InviteeLastName { get; set; } = default!;
        [DisplayName("Date Sent")] public DateTimeOffset InviteDate { get; set; }
        [DisplayName("Join Date")] public DateTimeOffset JoinDate { get; set; }
        [DisplayName("Code")] public Guid CompanyToken { get; set; }
        public bool IsValid { get; set; }

        // NP
        public virtual Company Company { get; set; } = default!;
        public virtual Project Project { get; set; } = default!;
        public virtual BTUser Invitor { get; set; } = default!;
        public virtual BTUser Invitee { get; set; } = default!;
    }
}
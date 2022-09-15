﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Models;

namespace TheBugTracker.Data
{
    /// <summary>
    /// This class is responsible for connecting to database.
    /// As the custom identity user was introduced, IdentityDbContext should accept BTUser type
    /// </summary>
    public sealed class ApplicationDbContext : IdentityDbContext<BTUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; } = default!;
        public DbSet<Invite> Invites { get; set; } = default!;
        public DbSet<Project> Projects { get; set; } = default!;
        public DbSet<Ticket> Tickets { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<ProjectPriority> ProjectPriorities { get; set; } = default!;
        public DbSet<TicketAttachment> TicketAttachments { get; set; } = default!;
        public DbSet<TicketComment> TicketComments { get; set; } = default!;
        public DbSet<TicketHistory> TicketHistories { get; set; } = default!;
        public DbSet<TicketPriority> TicketPriorities { get; set; } = default!;
        public DbSet<TicketStatus> TicketStatuses { get; set; } = default!;
        public DbSet<TicketType> TicketTypes { get; set; } = default!;
    }
}
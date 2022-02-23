using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheBugTrucker.Models;

namespace TheBugTrucker.Data
{
    /// <summary>
    /// This class is responsible for connecting to database.
    /// As the custom identity user was introduced, IdentityDbContext should accept BTUser type
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<BTUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
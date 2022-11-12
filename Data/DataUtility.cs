using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Npgsql;
using TheBugTracker.Models.Enums;
using TheBugTracker.Models;

namespace TheBugTracker.Data
{
    public static class DataUtility
    {
        //Company Ids
        private static int _accountingCorpId;
        private static int _softwareHouseId;

        public static string GetConnectionString(WebApplicationBuilder builder)
        {
            //The default connection string will come from appSettings like usual
            var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
            //It will be automatically overwritten if we are running on Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl)
                ? connectionString
                : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            //Provides a simple way to create and manage the contents of connection strings used by the NpgsqlConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public static async Task ManageDataAsync(IHost host)
        {
            using IServiceScope svcScope = host.Services.CreateScope();
            IServiceProvider svcProvider = svcScope.ServiceProvider;
            //Service: An instance of RoleManager
            ApplicationDbContext dbContextSvc =
                svcProvider.GetRequiredService<ApplicationDbContext>();
            //Service: An instance of RoleManager
            RoleManager<IdentityRole> roleManagerSvc = svcProvider.GetRequiredService<
                RoleManager<IdentityRole>
            >();
            //Service: An instance of the UserManager
            UserManager<BTUser> userManagerSvc = svcProvider.GetRequiredService<
                UserManager<BTUser>
            >();
            //Migration: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();

            //Custom  BugTracker Seed Methods
            await SeedRolesAsync(roleManagerSvc);
            await SeedDefaultCompaniesAsync(dbContextSvc);
            await SeedDefaultUsersAsync(userManagerSvc);
            // await SeedDemoUsersAsync(userManagerSvc);
            await SeedDefaultTicketTypeAsync(dbContextSvc);
            await SeedDefaultTicketStatusAsync(dbContextSvc);
            await SeedDefaultTicketPriorityAsync(dbContextSvc);
            await SeedDefaultProjectPriorityAsync(dbContextSvc);
            await SeedDefaultProjectsAsync(dbContextSvc);
            await SeedDefaultTicketsAsync(dbContextSvc);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.DemoUser.ToString()));
        }

        private static async Task SeedDefaultCompaniesAsync(ApplicationDbContext context)
        {
            try
            {
                IList<Company> defaultCompanies = new List<Company>
                {
                    new()
                    {
                        Name = "Accounting Corp",
                        Description = "Big finance corporation specialized in accounting services"
                    },
                    new()
                    {
                        Name = "Software House LLC",
                        Description =
                            "Small Software House that builds software for outside clients"
                    },
                };

                var dbCompanies = context.Companies.Select(c => c.Name).ToList();
                await context.Companies.AddRangeAsync(
                    defaultCompanies.Where(c => !dbCompanies.Contains(c.Name))
                );
                await context.SaveChangesAsync();

                //Get company Ids
                _accountingCorpId = context.Companies
                    .FirstOrDefault(p => p.Name == "Accounting Corp")
                    .Id;
                _softwareHouseId = context.Companies
                    .FirstOrDefault(p => p.Name == "Software House LLC")
                    .Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Companies.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultProjectPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<ProjectPriority> projectPriorities = new List<ProjectPriority>
                {
                    new() { Name = ProjectPriorities.Low.ToString() },
                    new() { Name = ProjectPriorities.Medium.ToString() },
                    new() { Name = ProjectPriorities.High.ToString() },
                    new() { Name = ProjectPriorities.Urgent.ToString() },
                };

                var dbProjectPriorities = context.ProjectPriorities.Select(c => c.Name).ToList();
                await context.ProjectPriorities.AddRangeAsync(
                    projectPriorities.Where(c => !dbProjectPriorities.Contains(c.Name))
                );
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Project Priorities.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultProjectsAsync(ApplicationDbContext context)
        {
            //Get project priority Ids
            int priorityLow = context.ProjectPriorities
                .FirstOrDefault(p => p.Name == ProjectPriorities.Low.ToString())
                .Id;
            int priorityMedium = context.ProjectPriorities
                .FirstOrDefault(p => p.Name == ProjectPriorities.Medium.ToString())
                .Id;
            int priorityHigh = context.ProjectPriorities
                .FirstOrDefault(p => p.Name == ProjectPriorities.High.ToString())
                .Id;
            int priorityUrgent = context.ProjectPriorities
                .FirstOrDefault(p => p.Name == ProjectPriorities.Urgent.ToString())
                .Id;

            var users = await context.Users.ToListAsync();
            var accountingAdmin = users.FirstOrDefault(u => u.FirstName == "John");

            var accountingDev1 = users.FirstOrDefault(u => u.FirstName == "Dave");
            var accountingDev2 = users.FirstOrDefault(u => u.FirstName == "Ann");
            
            var accountingSub1 = users.FirstOrDefault(u => u.FirstName == "Richard");
            var accountingSub2 = users.FirstOrDefault(u => u.FirstName == "Diana");
            
            
            var softwareHouseAdmin = users.FirstOrDefault(u => u.FirstName == "Daniel");

            var softwareHouseDev1 = users.FirstOrDefault(u => u.FirstName == "Maggie");
            var softwareHouseDev2 = users.FirstOrDefault(u => u.FirstName == "Cain");
            
            var softwareHouseSub1 = users.FirstOrDefault(u => u.FirstName == "Robert");
            var softwareHouseSub2 = users.FirstOrDefault(u => u.FirstName == "Eve");
            
            try
            {
                IList<Project> projects = new List<Project>
                {
                    new()
                    {
                        CompanyId = _accountingCorpId,
                        Name = "SAP S4 Hana implementation",
                        Description =
                            "Project that should implement SAP S4 Hana as the company's ERP system",
                        StartDate = new DateTimeOffset(DateTime.UtcNow),
                        EndDate = new DateTimeOffset(DateTime.UtcNow).AddMonths(3),
                        ProjectPriorityId = priorityLow,
                        Members = new List<BTUser>
                        {
                            accountingAdmin,
                            accountingDev1,
                            accountingDev2,
                            accountingSub1,
                            accountingSub2
                        }
                    },
                    new()
                    {
                        CompanyId = _softwareHouseId,
                        Name = "Build a social media application",
                        Description =
                            "Single page social app which allows users to chat and upload photos",
                        StartDate = new DateTimeOffset(DateTime.UtcNow),
                        EndDate = new DateTimeOffset(DateTime.UtcNow).AddMonths(3),
                        ProjectPriorityId = priorityMedium,
                        Members = new List<BTUser>
                        {
                            softwareHouseAdmin,
                            softwareHouseDev1,
                            softwareHouseDev2,
                            softwareHouseSub1,
                            softwareHouseSub2
                        }
                    },
                    new()
                    {
                        CompanyId = _accountingCorpId,
                        Name = "Year End Closing activities",
                        Description =
                            "Manage future Year End Closing activities - schedule tasks across teams",
                        StartDate = new DateTimeOffset(DateTime.UtcNow),
                        EndDate = new DateTimeOffset(DateTime.UtcNow).AddMonths(3),
                        ProjectPriorityId = priorityHigh,
                        Members = new List<BTUser>
                        {
                            accountingAdmin,
                            accountingDev1,
                            accountingDev2,
                            accountingSub1,
                            accountingSub2
                        }
                    },
                    new()
                    {
                        CompanyId = _softwareHouseId,
                        Name = "KPI metrics",
                        Description = "A project that tracks internal KPI metrics",
                        StartDate = new DateTimeOffset(DateTime.UtcNow),
                        EndDate = new DateTimeOffset(DateTime.UtcNow).AddMonths(12),
                        ProjectPriorityId = priorityLow,
                        Members = new List<BTUser>
                        {
                            softwareHouseAdmin,
                            softwareHouseDev1,
                            softwareHouseDev2,
                            softwareHouseSub1,
                            softwareHouseSub2
                        }
                    }
                };

                var dbProjects = context.Projects.Select(c => c.Name).ToList();
                await context.Projects.AddRangeAsync(
                    projects.Where(c => !dbProjects.Contains(c.Name))
                );
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Projects.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultUsersAsync(UserManager<BTUser> userManager)
        {
            //Seed Default Admin User
            var johnAdminUser = new BTUser
            {
                UserName = "JohnAdmin@bugtracker.com",
                Email = "JohnAdmin@bugtracker.com",
                FirstName = "John",
                LastName = "Admin",
                EmailConfirmed = true,
                CompanyId = _accountingCorpId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(johnAdminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(johnAdminUser, "Abc&123!");
                    await userManager.AddToRoleAsync(johnAdminUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding John Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Admin User
            var danielAdminUser = new BTUser
            {
                UserName = "DanielAdmin@bugtracker.com",
                Email = "DanielAdmin@bugtracker.com",
                FirstName = "Daniel",
                LastName = "Admin",
                EmailConfirmed = true,
                CompanyId = _softwareHouseId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(danielAdminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(danielAdminUser, "Abc&123!");
                    await userManager.AddToRoleAsync(danielAdminUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Daniel Admin User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default ProjectManager1 User
            var monicaPmUser = new BTUser
            {
                UserName = "MonicaPM@bugtracker.com",
                Email = "MonicaPM@bugtracker.com",
                FirstName = "Monica",
                LastName = "PM",
                EmailConfirmed = true,
                CompanyId = _accountingCorpId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(monicaPmUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(monicaPmUser, "Abc&123!");
                    await userManager.AddToRoleAsync(monicaPmUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Monica PM User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default ProjectManager2 User
            var marthaPmUser = new BTUser
            {
                UserName = "MarthaPM@bugtracker.com",
                Email = "MarthaPM@bugtracker.com",
                FirstName = "Martha",
                LastName = "PM",
                EmailConfirmed = true,
                CompanyId = _softwareHouseId,
                AvatarFormName = "",
                AvatarContentType = "",
                AvatarFileData = Array.Empty<byte>(),
            };
            try
            {
                var user = await userManager.FindByEmailAsync(marthaPmUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(marthaPmUser, "Abc&123!");
                    await userManager.AddToRoleAsync(marthaPmUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Martha PM User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default ProjectManager2 User
            var paulPmUser = new BTUser
            {
                UserName = "PaulPM@bugtracker.com",
                Email = "PaulPM@bugtracker.com",
                FirstName = "Paul",
                LastName = "PM",
                EmailConfirmed = true,
                CompanyId = _accountingCorpId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(paulPmUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(paulPmUser, "Abc&123!");
                    await userManager.AddToRoleAsync(paulPmUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Paul PM User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default ProjectManager2 User
            var ericPM2User = new BTUser
            {
                UserName = "EricPM@bugtracker.com",
                Email = "EricPM@bugtracker.com",
                FirstName = "Eric",
                LastName = "PM",
                EmailConfirmed = true,
                CompanyId = _softwareHouseId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(ericPM2User.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(ericPM2User, "Abc&123!");
                    await userManager.AddToRoleAsync(ericPM2User, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Default Eric PM User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Developer1 User
            var daveDev1User = new BTUser
            {
                UserName = "DaveDev@bugtracker.com",
                Email = "DaveDev@bugtracker.com",
                FirstName = "Dave",
                LastName = "Dev",
                EmailConfirmed = true,
                CompanyId = _accountingCorpId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(daveDev1User.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(daveDev1User, "Abc&123!");
                    await userManager.AddToRoleAsync(daveDev1User, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Dave Dev User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Developer2 User
            var maggieDev1 = new BTUser
            {
                UserName = "MaggieDev@bugtracker.com",
                Email = "MaggieDev@bugtracker.com",
                FirstName = "Maggie",
                LastName = "Dev",
                EmailConfirmed = true,
                CompanyId = _softwareHouseId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(maggieDev1.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(maggieDev1, "Abc&123!");
                    await userManager.AddToRoleAsync(maggieDev1, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Maggie Dev User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Developer2 User
            var addDev2User = new BTUser
            {
                UserName = "AnnDev@bugtracker.com",
                Email = "AnnDev@bugtracker.com",
                FirstName = "Ann",
                LastName = "Dev",
                EmailConfirmed = true,
                CompanyId = _accountingCorpId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(addDev2User.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(addDev2User, "Abc&123!");
                    await userManager.AddToRoleAsync(addDev2User, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ann Dev User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Developer2 User
            var cainDev2 = new BTUser
            {
                UserName = "CainDev2@bugtracker.com",
                Email = "CainDev2@bugtracker.com",
                FirstName = "Cain",
                LastName = "Dev",
                EmailConfirmed = true,
                CompanyId = _softwareHouseId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(cainDev2.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(cainDev2, "Abc&123!");
                    await userManager.AddToRoleAsync(cainDev2, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Cain Dev User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Submitter1 User
            var richardSub1User = new BTUser
            {
                UserName = "RichardSub@bugtracker.com",
                Email = "RichardSub@bugtracker.com",
                FirstName = "Richard",
                LastName = "Sub",
                EmailConfirmed = true,
                CompanyId = _accountingCorpId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(richardSub1User.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(richardSub1User, "Abc&123!");
                    await userManager.AddToRoleAsync(richardSub1User, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Richard Sub User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Submitter2 User
            var robertSub1User = new BTUser
            {
                UserName = "RobertSub@bugtracker.com",
                Email = "RobertSub@bugtracker.com",
                FirstName = "Robert",
                LastName = "Sub",
                EmailConfirmed = true,
                CompanyId = _softwareHouseId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(robertSub1User.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(robertSub1User, "Abc&123!");
                    await userManager.AddToRoleAsync(robertSub1User, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Robert Sub User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Submitter3 User
            var dianaSub2User = new BTUser
            {
                UserName = "DianaSub@bugtracker.com",
                Email = "DianaSub@bugtracker.com",
                FirstName = "Diana",
                LastName = "Sub",
                EmailConfirmed = true,
                CompanyId = _accountingCorpId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(dianaSub2User.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(dianaSub2User, "Abc&123!");
                    await userManager.AddToRoleAsync(dianaSub2User, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Diana Sub User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }

            //Seed Default Submitter4 User
            var eveSub2User = new BTUser
            {
                UserName = "EveSub@bugtracker.com",
                Email = "EveSub@bugtracker.com",
                FirstName = "Eve",
                LastName = "Sub",
                EmailConfirmed = true,
                CompanyId = _softwareHouseId,
            };
            try
            {
                var user = await userManager.FindByEmailAsync(eveSub2User.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(eveSub2User, "Abc&123!");
                    await userManager.AddToRoleAsync(eveSub2User, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Eve Sub User.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        // private static async Task SeedDemoUsersAsync(UserManager<BTUser> userManager)
        // {
        //     //Seed Demo Admin User
        //     var defaultUser = new BTUser
        //     {
        //         UserName = "demoadmin@bugtracker.com",
        //         Email = "demoadmin@bugtracker.com",
        //         FirstName = "Demo",
        //         LastName = "Admin",
        //         EmailConfirmed = true,
        //         CompanyId = accountingCorpId,
        //         AvatarFormName = "",
        //         AvatarContentType = "",
        //         AvatarFileData = Array.Empty<byte>(),
        //     };
        //     try
        //     {
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
        //             await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Demo Admin User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Demo ProjectManager User
        //     defaultUser = new BTUser
        //     {
        //         UserName = "demopm@bugtracker.com",
        //         Email = "demopm@bugtracker.com",
        //         FirstName = "Demo",
        //         LastName = "ProjectManager",
        //         EmailConfirmed = true,
        //         CompanyId = softwareHouseId,
        //         AvatarFormName = "",
        //         AvatarContentType = "",
        //         AvatarFileData = Array.Empty<byte>(),
        //     };
        //     try
        //     {
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
        //             await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Demo ProjectManager1 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Demo Developer User
        //     defaultUser = new BTUser
        //     {
        //         UserName = "demodev@bugtracker.com",
        //         Email = "demodev@bugtracker.com",
        //         FirstName = "Demo",
        //         LastName = "Developer",
        //         EmailConfirmed = true,
        //         CompanyId = softwareHouseId,
        //         AvatarFormName = "",
        //         AvatarContentType = "",
        //         AvatarFileData = Array.Empty<byte>(),
        //     };
        //     try
        //     {
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Developer.ToString());
        //             await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Demo Developer1 User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Demo Submitter User
        //     defaultUser = new BTUser
        //     {
        //         UserName = "demosub@bugtracker.com",
        //         Email = "demosub@bugtracker.com",
        //         FirstName = "Demo",
        //         LastName = "Submitter",
        //         EmailConfirmed = true,
        //         CompanyId = softwareHouseId,
        //         AvatarFormName = "",
        //         AvatarContentType = "",
        //         AvatarFileData = Array.Empty<byte>(),
        //     };
        //     try
        //     {
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
        //             await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Demo Submitter User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        //
        //
        //     //Seed Demo New User
        //     defaultUser = new BTUser
        //     {
        //         UserName = "demonew@bugtracker.com",
        //         Email = "demonew@bugtracker.com",
        //         FirstName = "Demo",
        //         LastName = "NewUser",
        //         EmailConfirmed = true,
        //         CompanyId = softwareHouseId,
        //         AvatarFormName = "",
        //         AvatarContentType = "",
        //         AvatarFileData = Array.Empty<byte>(),
        //     };
        //     try
        //     {
        //         var user = await userManager.FindByEmailAsync(defaultUser.Email);
        //         if (user == null)
        //         {
        //             await userManager.CreateAsync(defaultUser, "Abc&123!");
        //             await userManager.AddToRoleAsync(defaultUser, Roles.Submitter.ToString());
        //             await userManager.AddToRoleAsync(defaultUser, Roles.DemoUser.ToString());
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine("*************  ERROR  *************");
        //         Console.WriteLine("Error Seeding Demo New User.");
        //         Console.WriteLine(ex.Message);
        //         Console.WriteLine("***********************************");
        //         throw;
        //     }
        // }

        private static async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketType> ticketTypes = new List<TicketType>
                {
                    new() { Name = TicketTypes.NewDevelopment.ToString() }, // Ticket involves development of a new, un coded solution
                    new() { Name = TicketTypes.WorkTask.ToString() }, // Ticket involves development of the specific ticket description
                    new() { Name = TicketTypes.Defect.ToString() }, // Ticket involves unexpected development/maintenance on a previously designed feature/functionality
                    new() { Name = TicketTypes.ChangeRequest.ToString() }, // Ticket involves modification development of a previously designed feature/functionality
                    new() { Name = TicketTypes.Enhancement.ToString() }, // Ticket involves additional development on a previously designed feature or new functionality
                    new() { Name = TicketTypes.GeneralTask.ToString() } // Ticket involves no software development but may involve tasks such as configurations, or hardware setup
                };

                var dbTicketTypes = context.TicketTypes.Select(c => c.Name).ToList();
                await context.TicketTypes.AddRangeAsync(
                    ticketTypes.Where(c => !dbTicketTypes.Contains(c.Name))
                );
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Types.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketStatus> ticketStatuses = new List<TicketStatus>()
                {
                    new() { Name = TicketStatuses.New.ToString() }, // Newly Created ticket having never been assigned
                    new() { Name = TicketStatuses.Development.ToString() }, // Ticket is assigned and currently being worked
                    new() { Name = TicketStatuses.Testing.ToString() }, // Ticket is assigned and is currently being tested
                    new() { Name = TicketStatuses.Resolved.ToString() }, // Ticket remains assigned to the developer but work in now complete
                };

                var dbTicketStatuses = context.TicketStatuses.Select(c => c.Name).ToList();
                await context.TicketStatuses.AddRangeAsync(
                    ticketStatuses.Where(c => !dbTicketStatuses.Contains(c.Name))
                );
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Statuses.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                IList<TicketPriority> ticketPriorities = new List<TicketPriority>
                {
                    new() { Name = TicketPriorities.Low.ToString() },
                    new() { Name = TicketPriorities.Medium.ToString() },
                    new() { Name = TicketPriorities.High.ToString() },
                    new() { Name = TicketPriorities.Urgent.ToString() },
                };

                var dbTicketPriorities = context.TicketPriorities.Select(c => c.Name).ToList();
                await context.TicketPriorities.AddRangeAsync(
                    ticketPriorities.Where(c => !dbTicketPriorities.Contains(c.Name))
                );
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Ticket Priorities.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }

        private static async Task SeedDefaultTicketsAsync(ApplicationDbContext context)
        {
            var users = await context.Users.ToListAsync();

            var accountingSub1 = users.FirstOrDefault(u => u.FirstName == "Richard");
            var accountingSub2 = users.FirstOrDefault(u => u.FirstName == "Diana");

            var softwareHouseSub1 = users.FirstOrDefault(u => u.FirstName == "Robert");
            var softwareHouseSub2 = users.FirstOrDefault(u => u.FirstName == "Eve");
            
            //Get project Ids
            int s4HanaId = context.Projects
                .FirstOrDefault(p => p.Name == "SAP S4 Hana implementation")
                .Id;
            int socialAppId = context.Projects
                .FirstOrDefault(p => p.Name == "Build a social media application")
                .Id;
            int yecId = context.Projects
                .FirstOrDefault(p => p.Name == "Year End Closing activities")
                .Id;
            int kpiId = context.Projects.FirstOrDefault(p => p.Name == "KPI metrics").Id;

            //Get ticket type Ids
            int typeNewDev = context.TicketTypes
                .FirstOrDefault(p => p.Name == TicketTypes.NewDevelopment.ToString())
                .Id;
            int typeWorkTask = context.TicketTypes
                .FirstOrDefault(p => p.Name == TicketTypes.WorkTask.ToString())
                .Id;
            int typeDefect = context.TicketTypes
                .FirstOrDefault(p => p.Name == TicketTypes.Defect.ToString())
                .Id;
            int typeEnhancement = context.TicketTypes
                .FirstOrDefault(p => p.Name == TicketTypes.Enhancement.ToString())
                .Id;
            int typeChangeRequest = context.TicketTypes
                .FirstOrDefault(p => p.Name == TicketTypes.ChangeRequest.ToString())
                .Id;

            //Get ticket priority Ids
            int priorityLow = context.TicketPriorities
                .FirstOrDefault(p => p.Name == TicketPriorities.Low.ToString())
                .Id;
            int priorityMedium = context.TicketPriorities
                .FirstOrDefault(p => p.Name == TicketPriorities.Medium.ToString())
                .Id;
            int priorityHigh = context.TicketPriorities
                .FirstOrDefault(p => p.Name == TicketPriorities.High.ToString())
                .Id;
            int priorityUrgent = context.TicketPriorities
                .FirstOrDefault(p => p.Name == TicketPriorities.Urgent.ToString())
                .Id;

            //Get ticket status Ids
            int statusNew = context.TicketStatuses
                .FirstOrDefault(p => p.Name == TicketStatuses.New.ToString())
                .Id;
            int statusDev = context.TicketStatuses
                .FirstOrDefault(p => p.Name == TicketStatuses.Development.ToString())
                .Id;
            int statusTest = context.TicketStatuses
                .FirstOrDefault(p => p.Name == TicketStatuses.Testing.ToString())
                .Id;
            int statusResolved = context.TicketStatuses
                .FirstOrDefault(p => p.Name == TicketStatuses.Resolved.ToString())
                .Id;
            try
            {
                IList<Ticket> tickets = new List<Ticket>()
                {
                    //PORTFOLIO
                    new()
                    {
                        Title = "S4 sandbox access request",
                        Description = "I need access to new sandbox environment",
                        Created = DateTimeOffset.Now,
                        ProjectId = s4HanaId,
                        TicketPriorityId = priorityLow,
                        TicketStatusId = statusNew,
                        TicketTypeId = typeChangeRequest,
                        OwnerUserId = accountingSub1.Id,
                        OwnerUser = accountingSub1
                    },
                    new()
                    {
                        Title = "Cost Center mapping is incorrect",
                        Description = "Payroll cost accounts for EMEA are incorrectly mapped",
                        Created = DateTimeOffset.Now,
                        ProjectId = s4HanaId,
                        TicketPriorityId = priorityMedium,
                        TicketStatusId = statusNew,
                        TicketTypeId = typeDefect,
                        OwnerUserId = accountingSub2.Id,
                        OwnerUser = accountingSub2
                    },
                    new()
                    {
                        Title = "New feature request",
                        Description = "Request for real time chat feature",
                        Created = DateTimeOffset.Now,
                        ProjectId = socialAppId,
                        TicketPriorityId = priorityHigh,
                        TicketStatusId = statusDev,
                        TicketTypeId = typeEnhancement,
                        OwnerUserId = softwareHouseSub1.Id,
                        OwnerUser = softwareHouseSub1
                    },
                    new()
                    {
                        Title = "Send message malfunction",
                        Description = "I cannot send a private message to the user",
                        Created = DateTimeOffset.Now,
                        ProjectId = socialAppId,
                        TicketPriorityId = priorityUrgent,
                        TicketStatusId = statusTest,
                        TicketTypeId = typeDefect,
                        OwnerUserId = softwareHouseSub2.Id,
                        OwnerUser = softwareHouseSub2
                    },
                    new()
                    {
                        Title = "Payroll costs analysis",
                        Description =
                            "Payroll costs for Germany are higher than anticipated- please provide more details",
                        Created = DateTimeOffset.Now,
                        ProjectId = yecId,
                        TicketPriorityId = priorityLow,
                        TicketStatusId = statusNew,
                        TicketTypeId = typeNewDev,
                        OwnerUserId = accountingSub1.Id,
                        OwnerUser = accountingSub1
                    },
                    new()
                    {
                        Title = "YEC schedule",
                        Description = "Provide schedule for french YES activities",
                        Created = DateTimeOffset.Now,
                        ProjectId = yecId,
                        TicketPriorityId = priorityMedium,
                        TicketStatusId = statusNew,
                        TicketTypeId = typeChangeRequest,
                        OwnerUserId = accountingSub2.Id,
                        OwnerUser = accountingSub2
                    },
                    new()
                    {
                        Title = "KPI bad results",
                        Description = "KPI results are lower than anticipated",
                        Created = DateTimeOffset.Now,
                        ProjectId = kpiId,
                        TicketPriorityId = priorityHigh,
                        TicketStatusId = statusDev,
                        TicketTypeId = typeEnhancement,
                        OwnerUserId = softwareHouseSub1.Id,
                        OwnerUser = softwareHouseSub1
                    },
                    new()
                    {
                        Title = "Feedback request for next milestone",
                        Description = "Next milestone for social media app is approaching",
                        Created = DateTimeOffset.Now,
                        ProjectId = kpiId,
                        TicketPriorityId = priorityUrgent,
                        TicketStatusId = statusTest,
                        TicketTypeId = typeDefect,
                        OwnerUserId = softwareHouseSub2.Id,
                        OwnerUser = softwareHouseSub2
                    },
                };

                var dbTickets = context.Tickets.Select(c => c.Title).ToList();
                await context.Tickets.AddRangeAsync(
                    tickets.Where(c => !dbTickets.Contains(c.Title))
                );
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("*************  ERROR  *************");
                Console.WriteLine("Error Seeding Tickets.");
                Console.WriteLine(ex.Message);
                Console.WriteLine("***********************************");
                throw;
            }
        }
    }
}

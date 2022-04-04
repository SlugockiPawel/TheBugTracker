#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBugTrucker.Data;
using TheBugTrucker.Extensions;
using TheBugTrucker.Models;
using TheBugTrucker.Models.Enums;
using TheBugTrucker.Models.ViewModels;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRolesService _rolesService;
        private readonly ILookupService _lookupsService;
        private readonly IFileService _fileService;
        private readonly IProjectService _projectService;

        public ProjectsController(ApplicationDbContext context, IRolesService rolesService,
            ILookupService lookupsService, IFileService fileService, IProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _lookupsService = lookupsService;
            _fileService = fileService;
            _projectService = projectService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            // Add model for Create View
            AddProjectWithPMViewModel model = new();

            // Load model SelectLists with data
            model.PMList =
                new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId),
                    "Id", "FullName");

            model.PriorityList = new SelectList(await _lookupsService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if (model is not null)
            {
                int companyId = User.Identity.GetCompanyId().Value;

                try
                {
                    if (model.Project.FormFile is not null)
                    {
                        model.Project.FileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.FormFile);
                        model.Project.FileName = model.Project.FormFile.FileName;
                        model.Project.FileContentType = model.Project.FormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;

                    await _projectService.AddNewProjectAsync(model.Project);

                    // Add PM if one was chosen
                    if (!string.IsNullOrWhiteSpace(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }

                // TODO Redirect to All Projects view
                return RedirectToAction("Index");
            }

            return RedirectToAction("Create");
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            // Add model for Create View
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            // Load model SelectLists with data
            model.PMList =
                new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId),
                    "Id", "FullName");

            model.PriorityList = new SelectList(await _lookupsService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model is not null)
            {
                try
                {
                    if (model.Project.FormFile is not null)
                    {
                        model.Project.FileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.FormFile);
                        model.Project.FileName = model.Project.FormFile.FileName;
                        model.Project.FileContentType = model.Project.FormFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(model.Project);

                    // Add PM if one was chosen
                    if (!string.IsNullOrWhiteSpace(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            return RedirectToAction("Edit");
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);


            if (project is null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Project project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        // GET: Projects/Restore/5
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);


            if (project is null)
            {
                return NotFound();
            }

            return View(project);
        }
        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
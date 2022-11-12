using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.ChartModels;
using TheBugTracker.Models.Enums;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers;

public sealed class HomeController : Controller
{
    private readonly ICompanyInfoService _companyInfoService;
    private readonly ILogger<HomeController> _logger;
    private readonly IProjectService _projectService;
    private readonly SignInManager<BTUser> _signInManager;
    private readonly UserManager<BTUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ICompanyInfoService companyInfoService,
        IProjectService projectService, SignInManager<BTUser> signInManager, UserManager<BTUser> userManager)
    {
        _logger = logger;
        _companyInfoService = companyInfoService;
        _projectService = projectService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DemoLogin(string userName)
    {
        if (User.Identity.IsAuthenticated)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        var user = userName switch
        {
            "John" => await _userManager.FindByEmailAsync("JohnAdmin@bugtracker.com"),
            "Monica" => await _userManager.FindByEmailAsync("MonicaPM@bugtracker.com"),
            "Dave" => await _userManager.FindByEmailAsync("DaveDev@bugtracker.com"),
            "Diana" => await _userManager.FindByEmailAsync("DianaSub@bugtracker.com"),
            _ => null
        };

        if (user is not null)
        {
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Dashboard");
        }

        // something went wrong- redisplay the page
        return RedirectToAction("Index");
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        DashboardViewModel model = new();
        var companyId = User.Identity.GetCompanyId().Value;

        model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
        model.Projects = (await _companyInfoService.GetAllProjectsAsync(companyId))
            .Where(p => p.Archived == false)
            .ToList();

        model.Tickets = model.Projects.SelectMany(p => p.Tickets)
            .Where(t => t.Archived == false)
            .ToList();

        model.Members = model.Company.Members.ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> GglProjectPriority()
    {
        var companyId = User.Identity.GetCompanyId().Value;

        var projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

        List<object> chartData = new();
        chartData.Add(new object[] { "Priority", "Count" });


        foreach (var priority in Enum.GetNames(typeof(ProjectPriorities)))
        {
            var priorityCount = (await _projectService.GetAllProjectsByPriorityAsync(companyId, priority)).Count();
            chartData.Add(new object[] { priority, priorityCount });
        }

        return Json(chartData);
    }

    [HttpPost]
    public async Task<JsonResult> GglProjectTickets()
    {
        var companyId = User.Identity.GetCompanyId().Value;

        var projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

        List<object> chartData = new();
        chartData.Add(new object[] { "ProjectName", "TicketCount" });

        foreach (var prj in projects)
        {
            chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
        }

        return Json(chartData);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<JsonResult> AmCharts()
    {
        AmChartData amChartData = new();
        List<AmItem> amItems = new();

        var companyId = User.Identity.GetCompanyId().Value;

        var projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false)
            .ToList();

        foreach (var project in projects)
        {
            AmItem item = new();

            item.Project = project.Name;
            item.Tickets = project.Tickets.Count;
            item.Developers = (await _projectService.GetProjectMembersByRoleAsync(project.Id, nameof(Roles.Developer)))
                .Count();

            amItems.Add(item);
        }

        amChartData.Data = amItems.ToArray();


        return Json(amChartData.Data);
    }

    [HttpPost]
    public async Task<JsonResult> PlotlyBarChart()
    {
        PlotlyBarData plotlyData = new();
        List<PlotlyBar> barData = new();

        var companyId = User.Identity.GetCompanyId().Value;

        var projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

        //Bar One
        PlotlyBar barOne = new()
        {
            X = projects.Select(p => p.Name).ToArray(),
            Y = projects.SelectMany(p => p.Tickets).GroupBy(t => t.ProjectId).Select(g => g.Count()).ToArray(),
            Name = "Tickets",
            Type = "bar"
        };

        //Bar Two
        PlotlyBar barTwo = new()
        {
            X = projects.Select(p => p.Name).ToArray(),
            Y = projects
                .Select(async p =>
                    (await _projectService.GetProjectMembersByRoleAsync(p.Id, nameof(Roles.Developer))).Count)
                .Select(c => c.Result).ToArray(),
            Name = "Developers",
            Type = "bar"
        };

        barData.Add(barOne);
        barData.Add(barTwo);

        plotlyData.Data = barData;

        return Json(plotlyData);
    }
}
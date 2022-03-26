using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly IRolesService _rolesService;
        private readonly ICompanyInfoService _companyInfoService;

        public UserRolesController(IRolesService rolesService, ICompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        public async Task<IActionResult> ManageUserRoles()
        {
            return View();
        }
    }
}

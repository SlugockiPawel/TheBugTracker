using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IRolesService _rolesService;
        private readonly ICompanyInfoService _companyInfoService;

        public UserRolesController(IRolesService rolesService, ICompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            // Get all company users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            // Populate the ViewModel
            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            BTUser user = (await _companyInfoService.GetAllMembersAsync(companyId))
                .FirstOrDefault(u => u.Id == member.BTUser.Id);

            //Get roles for the user
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(user);

            //Grab the selected roles
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                //Remove from their roles
                if (await _rolesService.RemoveUserFromRolesAsync(user, roles))
                {
                    //Add user to new role
                    await _rolesService.AddUserToRoleAsync(user, userRole);
                }
            }

            //Navigate back to the view
            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
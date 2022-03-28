using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using TheBugTrucker.Extensions;
using TheBugTrucker.Models;
using TheBugTrucker.Models.ViewModels;
using TheBugTrucker.Services.Interfaces;

namespace TheBugTrucker.Controllers
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
            //Add an instance of the ViewModel as a list
            List<ManageUserRolesViewModel> model = new();

            //Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Get all company users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            //Loop over the users to populate the ViewModel
            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                // TODO make multiselect select currrent roles for each user
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(),  selected);

                model.Add(viewModel);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            //Get the company Id
            int companyId = User.Identity.GetCompanyId().Value;

            //Instantiate user
            BTUser bugTrackerUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);

            //Get roles for the user
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(bugTrackerUser);

            //Grab the selected roles
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                //Remove from their roles
                if (await _rolesService.RemoveUserFromRolesAsync(bugTrackerUser, roles))
                {
                    //Add user to new role
                    await _rolesService.AddUserToRoleAsync(bugTrackerUser, userRole);
                }
            }

            //Navigate back to the view
            return RedirectToAction(nameof(ManageUserRoles));

        }
    }
}
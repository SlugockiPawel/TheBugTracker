using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheBugTrucker.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BTUser BTUser { get; set; } = default!;
        public MultiSelectList Roles { get; set; } = default!;
        public List<string> SelectedRoles { get; set; } = new();
    }
}

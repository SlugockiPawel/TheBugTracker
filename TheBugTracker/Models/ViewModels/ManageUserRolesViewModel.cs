using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheBugTracker.Models.ViewModels
{
    public sealed class ManageUserRolesViewModel
    {
        public BTUser BTUser { get; set; } = default!;
        public MultiSelectList Roles { get; set; } = default!;
        public List<string> SelectedRoles { get; set; } = default!;
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheBugTracker.Models.ViewModels
{
    public sealed class AddProjectWithPMViewModel
    {
        public Project Project { get; set; } = default!;
        public SelectList PMList { get; set; } = default!;
        public string PmId { get; set; } = default!;
        public SelectList PriorityList { get; set; } = default!;
    }
}

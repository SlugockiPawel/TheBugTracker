using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheBugTrucker.Models.ViewModels
{
    public class AddProjectWithPMViewModel
    {
        public Project Project { get; set; } = default!;
        public SelectList PMList { get; set; } = default!;
        public string PmId { get; set; } = default!;
        public SelectList PriorityList { get; set; } = default!;
    }
}

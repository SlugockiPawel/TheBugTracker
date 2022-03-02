using System.ComponentModel;

namespace TheBugTrucker.Models
{
    public class ProjectPriority
    {
        public int Id { get; set; }

        [DisplayName("Priority Name")]
        public string Name { get; set; } = default!;
    }
}

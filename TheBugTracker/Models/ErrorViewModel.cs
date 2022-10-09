#nullable enable
namespace TheBugTracker.Models
{
    public sealed class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
namespace TheBugTracker.Models.ChartModels
{
    public sealed class AmChartData
    {
        public AmItem[] Data { get; set; }
    }

    public sealed class AmItem
    {
        public string Project { get; set; }
        public int Tickets { get; set; }
        public int Developers { get; set; }
    }
}

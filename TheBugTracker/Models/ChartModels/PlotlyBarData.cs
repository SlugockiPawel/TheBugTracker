namespace TheBugTracker.Models.ChartModels
{
    public sealed class PlotlyBarData
    {
        public List<PlotlyBar> Data { get; set; }
    }

    public sealed class PlotlyBar
    {
        public string[] X { get; set; }
        public int[] Y { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
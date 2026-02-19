namespace IOTWave.Models;

public class StatuSeriesGroup : DataSeriesGroupBase
{
    public Dictionary<int, string> Descriptions { get; set; } = new();
    public List<StatuPoint> Points { get; set; } = new();
    public Dictionary<int, IBrush> Brushes { get; set; } = new();
}
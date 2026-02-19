namespace IOTWave.Models;

public class TimeRangeMarker
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Label { get; set; } = string.Empty;
    public Color Color { get; set; } = Color.FromArgb(60, 255, 0, 0);
    public bool IsVisible { get; set; } = true;
}
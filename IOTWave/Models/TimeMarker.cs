namespace IotWave.Models;

public class TimeMarker
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Time { get; set; }
    public string Label { get; set; } = string.Empty;
    public Color Color { get; set; } = Colors.Red;
    public bool IsVisible { get; set; } = true;
}
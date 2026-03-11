using CommunityToolkit.Mvvm.ComponentModel;

namespace IOTWave.Models;

public partial class DataSeriesGroupBase:ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public string Legend { get; set; } = string.Empty;
    public int Length { get; set; } = 24;
    public Object DataSource { get; set; }=string.Empty;
    public Object Tags { get; set; } = string.Empty;
    public int Height { get; set; }

    [ObservableProperty]
    private bool _isVisible = true;  

    public Dictionary<string, object> Properties { get; set; } = new();

}
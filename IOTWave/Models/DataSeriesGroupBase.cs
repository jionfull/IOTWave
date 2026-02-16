namespace IotWave.Models;

public class DataSeriesGroupBase
{
    public string Name { get; set; } = string.Empty;
    public string Legend { get; set; } = string.Empty;
    public int Length { get; set; } = 24;
    public Object DataSource { get; set; }=string.Empty;
    public Object Tags { get; set; } = string.Empty;
    public int Height { get; set; }

}
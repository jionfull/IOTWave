namespace IOTWave.Models;

public class CurveGroup : DataSeriesGroupBase
{

    public List<CurveData> Curves { get; set; } = new();
    public List<YMarker> YMarkers { get; set; } = new();

}
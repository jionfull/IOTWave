using System.Collections.Generic;

namespace IOTWave.Models;
// DataModels.cs
public class TimePoint
{
    public DateTime Time { get; set; }
    public double Value { get; set; }

    public static class TimeComparer
    {
        public static readonly IComparer<TimePoint> Instance = new TimePointComparer();

        private class TimePointComparer : IComparer<TimePoint>
        {
            public int Compare(TimePoint x, TimePoint y)
            {
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                return x.Time.CompareTo(y.Time);
            }
        }
    }
}

public class StatuPoint
{
    public DateTime Time { get; set; }
    public int Value { get; set; }
}

public class PanelConfiguration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public PanelType Type { get; set; } = PanelType.Curve;
   
    public bool ShowGrid { get; set; } = true;
    public bool ShowLegend { get; set; } = true;
    public Color GridColor { get; set; } = Color.FromArgb(80, 0, 180, 216);
 
  
}

public enum PanelType
{
    Curve,      // 曲线显示面板
    Status      // 状态显示面板
}
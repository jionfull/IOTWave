namespace IOTWave.Models
{
    /// <summary>
    /// Y轴标记，由Y轴值、标题和标签构成
    /// </summary>
    public class YMarker
    {
        public double Value { get; set; }
        public string Caption { get; set; } = string.Empty;
        public object? Tag { get; set; }

        public YMarker(double value, string caption, object? tag = null)
        {
            Value = value;
            Caption = caption;
            Tag = tag;
        }
    }
}
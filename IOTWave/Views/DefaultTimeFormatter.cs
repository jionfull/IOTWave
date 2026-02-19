namespace IOTWave.Views;

public class DefaultTimeFormatter : ITimeFormatter
{
    public string FormatTime(double timeInSeconds)
    {
        if (timeInSeconds < 1e-3) // 微秒范围
        {
            return $"{(timeInSeconds * 1e6):0}µs";
        }
        else if (timeInSeconds < 1) // 毫秒范围
        {
            return $"{(timeInSeconds * 1e3):0.#}ms";
        }
        else if (timeInSeconds < 60) // 秒范围
        {
            return $"{timeInSeconds:0.##}s";
        }
        else if (timeInSeconds < 3600) // 分钟范围
        {
            var minutes = timeInSeconds / 60;
            return $"{minutes:0.#}m";
        }
        else // 小时范围
        {
            var hours = timeInSeconds / 3600;
            return $"{hours:0.#}h";
        }
    }
    
    
}
namespace IotWave.Views;

public interface ITimeFormatter
{
    string FormatTime(double timeInSeconds);
}

// 科学计数法格式化器
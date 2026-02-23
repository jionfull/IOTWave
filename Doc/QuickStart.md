# IOTWave 快速入门教程

## 1. 创建新项目

首先创建一个新的 Avalonia 应用程序项目：

```bash
dotnet new avalonia.app -n IOTWaveDemo
cd IOTWaveDemo
```

## 2. 安装 IOTWave 包

```bash
dotnet add package IOTWave
```

## 3. 更新 MainWindow.xaml

打开 `MainWindow.xaml` 文件并替换成以下内容：

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:iotwave="clr-namespace:IOTWave.Views;assembly=IOTWave"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="IOTWaveDemo.MainWindow"
        Title="IOTWave Demo">

  <ScrollViewer>
    <iotwave:WaveListPanel x:Name="WavePanel" />
  </ScrollViewer>
</Window>
```

## 4. 更新 MainWindow.xaml.cs

```csharp
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IOTWave.Models;
using IOTWave.Views;
using Avalonia.Media;
using System;

namespace IOTWaveDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // 初始化波形面板
        InitializeWavePanel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializeWavePanel()
    {
        var wavePanel = this.FindControl<WaveListPanel>("WavePanel");

        // 设置时间范围（最近1小时）
        var endTime = DateTime.Now;
        var startTime = endTime.AddHours(-1);

        wavePanel.StartTime = startTime;
        wavePanel.EndTime = endTime;
        wavePanel.AutoDistributePanelHeight = true;

        // 创建传感器数据
        var sensorData = GenerateSampleData(startTime, endTime);

        // 设置数据源
        wavePanel.ItemsSource = sensorData;
    }

    private List<object> GenerateSampleData(DateTime startTime, DateTime endTime)
    {
        var data = new List<object>();

        // 创建温度曲线
        var tempCurve = new CurveData
        {
            Name = "Temperature",
            Color = Colors.Red,
            ShowPoints = true,
            LineWidth = 2
        };

        // 生成温度数据（模拟正弦波加噪声）
        var timeSpan = (endTime - startTime).TotalSeconds;
        for (int i = 0; i < 3600; i++) // 每秒一个点
        {
            var currentTime = startTime.AddSeconds(i);
            var value = 20 + 10 * Math.Sin(i * 0.01) + Random.Shared.NextDouble() * 2;

            tempCurve.Points.Add(new TimePoint
            {
                Time = currentTime,
                Value = value
            });
        }

        // 创建压力曲线
        var pressureCurve = new CurveData
        {
            Name = "Pressure",
            Color = Colors.Blue,
            ShowPoints = false,
            LineWidth = 1.5
        };

        // 生成压力数据
        for (int i = 0; i < 3600; i++)
        {
            var currentTime = startTime.AddSeconds(i);
            var value = 1013.25 + 5 * Math.Cos(i * 0.008) + Random.Shared.NextDouble();

            pressureCurve.Points.Add(new TimePoint
            {
                Time = currentTime,
                Value = value
            });
        }

        // 创建湿度曲线
        var humidityCurve = new CurveData
        {
            Name = "Humidity",
            Color = Colors.Green,
            ShowPoints = true,
            LineWidth = 1
        };

        // 生成湿度数据
        for (int i = 0; i < 3600; i++)
        {
            var currentTime = startTime.AddSeconds(i);
            var value = 50 + 15 * Math.Sin(i * 0.005) + Random.Shared.NextDouble() * 5;

            humidityCurve.Points.Add(new TimePoint
            {
                Time = currentTime,
                Value = Math.Clamp(value, 0, 100) // 湿度限制在0-100之间
            });
        }

        // 创建温度曲线组
        var tempGroup = new CurveGroup
        {
            Name = "Temperature Data",
            Curves = new List<CurveData> { tempCurve },
            YMarkers = new List<YMarker>
            {
                new YMarker(30, "High Alert"),
                new YMarker(10, "Low Alert")
            }
        };

        // 创建压力和湿度组合
        var pressureHumidityGroup = new CurveGroup
        {
            Name = "Environmental Data",
            Curves = new List<CurveData> { pressureCurve, humidityCurve },
            YMarkers = new List<YMarker>
            {
                new YMarker(1020, "Pressure Max"),
                new YMarker(30, "Humidity Min")
            }
        };

        // 添加到数据列表
        data.Add(tempGroup);
        data.Add(pressureHumidityGroup);

        return data;
    }
}
```

## 5. 运行应用程序

```bash
dotnet run
```

## 6. 交互功能

一旦运行应用程序，您可以：

- **滚动鼠标滚轮**: 在时间轴上放大/缩小
- **拖拽**: 平移查看不同的时间段
- **Ctrl + 滚轮**: 在Y轴方向上缩放
- **Ctrl + 拖拽**: 在Y轴方向上平移
- **悬停**: 查看特定时间点的数值

## 7. 自定义配置

### 7.1 添加Y轴标记

```csharp
var curveGroup = new CurveGroup
{
    Name = "My Sensor",
    Curves = new List<CurveData> { myCurve },
    YMarkers = new List<YMarker>
    {
        new YMarker(100, "Upper Limit", "LIMIT_UPPER"),
        new YMarker(50, "Target", "TARGET"),
        new YMarker(20, "Lower Limit", "LIMIT_LOWER")
    }
};
```

### 7.2 启用相对时间显示

```csharp
wavePanel.UseRelativeTime = true;
wavePanel.RelativeTimeBase = DateTime.Now.AddHours(-1); // 设置基准时间
```

### 7.3 自动分布面板高度

```csharp
wavePanel.AutoDistributePanelHeight = true; // 自动平均分配高度
```

## 8. 高级用法

### 8.1 实时数据更新

```csharp
// 添加新的数据点到现有曲线
var newPoint = new TimePoint
{
    Time = DateTime.Now,
    Value = newValue
};
existingCurve.Points.Add(newPoint);

// 视觉刷新
wavePanel.InvalidateVisual();
```

### 8.2 时间范围动态调整

```csharp
// 实现跟随最新数据的时间窗口
private void UpdateTimeWindow()
{
    var endTime = DateTime.Now;
    var startTime = endTime.AddMinutes(-10); // 显示最近10分钟

    wavePanel.StartTime = startTime;
    wavePanel.EndTime = endTime;
}

// 每秒更新一次
DispatcherTimer.Run(() =>
{
    UpdateTimeWindow();
    return true; // 继续运行定时器
}, TimeSpan.FromSeconds(1));
```

### 8.3 添加时间标记

```csharp
// 虽然直接添加TimeMarker在WaveListPanel中不是直接支持的，
// 但可以通过在CurveGroup中添加特殊标记来实现类似效果

// 在曲线数据中标识重要事件
foreach (var curve in curveGroup.Curves)
{
    // 在特定时间点添加特殊标记
    var eventTime = DateTime.Now.AddSeconds(-30);
    // 实现自定义标记逻辑...
}
```

## 9. 故障排除

### 9.1 常见问题

**数据不显示**:
- 检查时间范围是否与数据时间匹配
- 确认曲线颜色是否与背景色相同
- 验证数据点是否有效（非NaN）

**性能问题**:
- 减少数据点数量或使用数据聚合
- 关闭 ShowPoints 属性以提高性能
- 适当限制时间轴的显示范围

**UI响应慢**:
- 考虑实现虚拟化或分页加载
- 减少同时显示的曲线数量

## 10. 下一步

现在您已经创建了一个基本的 IOTWave 应用程序，可以进一步探索：

- 添加更多传感器类型
- 实现数据导出功能
- 添加报警和通知功能
- 集成后端数据源
- 实现用户界面定制

享受使用 IOTWave 构建您的 IoT 数据可视化应用！
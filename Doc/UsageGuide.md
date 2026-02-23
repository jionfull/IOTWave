# IOTWave 使用手册

## 1. 概述

IOTWave是一个基于 .NET 8.0 和 .NET 10 的物联网(IoT)波形显示和数据处理框架库。它提供了高性能的波形显示控件、时间轴标记功能和灵活的数据可视化能力。

## 2. 安装

### 2.1 通过 NuGet 包管理器安装

```bash
dotnet add package IOTWave
```

或者通过包管理器控制台：

```powershell
Install-Package IOTWave
```

### 2.2 通过项目文件添加

在 `.csproj` 文件中添加以下内容：

```xml
<PackageReference Include="IOTWave" Version="1.0.0" />
```

## 3. 快速开始

### 3.1 基本用法

#### 3.1.1 创建波形面板

```csharp
using IOTWave.Views;
using IOTWave.Models;

// 创建 WaveListPanel
var wavePanel = new WaveListPanel
{
    StartTime = DateTime.Now.AddHours(-1),
    EndTime = DateTime.Now,
    AutoDistributePanelHeight = true
};

// 添加曲线数据
wavePanel.ItemsSource = curveGroups;
```

#### 3.1.2 准备数据

```csharp
// 创建曲线数据
var curveData = new CurveData
{
    Name = "Temperature",
    Color = Colors.Red,
    Points = new List<TimePoint>
    {
        new TimePoint { Time = DateTime.Now.AddSeconds(-10), Value = 25.5 },
        new TimePoint { Time = DateTime.Now.AddSeconds(-5), Value = 26.0 },
        new TimePoint { Time = DateTime.Now, Value = 26.2 }
    }
};

// 创建曲线组
var curveGroup = new CurveGroup
{
    Name = "Sensor Data",
    Curves = new List<CurveData> { curveData }
};

// 将曲线组添加到面板
var curveGroups = new List<object> { curveGroup };
wavePanel.ItemsSource = curveGroups;
```

## 4. 核心组件详解

### 4.1 CurveData - 曲线数据模型

`CurveData` 表示一条时间序列曲线数据：

| 属性 | 类型 | 描述 |
|------|------|------|
| Id | string | 曲线唯一标识符 |
| Name | string | 曲线名称 |
| Legend | string | 图例文本 |
| Color | Color | 曲线颜色 |
| Brush | IBrush | 曲线画刷（根据颜色自动生成） |
| Points | List<TimePoint> | 时间点数据集合 |
| MinValue | double | 最小值（只读） |
| MaxValue | double | 最大值（只读） |
| ShowPoints | bool | 是否显示数据点 |
| LineWidth | double | 线条宽度 |
| PointShowLimit | double | 数据点显示间隔阈值 |
| IsVisible | bool | 是否可见 |

### 4.2 CurveGroup - 曲线组模型

`CurveGroup` 代表一组相关的曲线和Y轴标记：

```csharp
public class CurveGroup : DataSeriesGroupBase
{
    public List<CurveData> Curves { get; set; } = new();
    public List<YMarker> YMarkers { get; set; } = new();
}
```

### 4.3 TimePoint - 时间点模型

`TimePoint` 表示一个带有时间戳的数据点：

```csharp
public class TimePoint
{
    public DateTime Time { get; set; }
    public double Value { get; set; }

    // 内置时间比较器
    public static TimePointComparer Instance { get; }
}
```

### 4.4 YMarker - Y轴标记

`YMarker` 表示Y轴上的水平标记线：

```csharp
public class YMarker
{
    public double Value { get; set; }      // Y轴值
    public string Caption { get; set; }    // 标题
    public object? Tag { get; set; }       // 自定义标签
}
```

## 5. 主要控件

### 5.1 WaveListPanel - 波形列表面板

`WaveListPanel` 是主容器控件，用于展示多个波形面板：

| 属性 | 类型 | 描述 |
|------|------|------|
| ItemSource | IEnumerable | 数据源 |
| StartTime | DateTime | 开始时间 |
| EndTime | DateTime | 结束时间 |
| AutoDistributePanelHeight | bool | 是否自动分配面板高度 |
| StatusPanelHeight | int | 状态面板高度 |
| UseRelativeTime | bool | 是否使用相对时间 |

### 5.2 CurvePanel - 曲线面板

`CurvePanel` 用于显示单个曲线：

| 属性 | 类型 | 描述 |
|------|------|------|
| Items | CurveGroup | 曲线组数据 |
| YMarkers | IList<YMarker> | Y轴标记 |
| ShowYAxis | bool | 是否显示Y轴 |
| DesiredHeight | double | 期望高度 |

## 6. 高级功能

### 6.1 时间轴标记

支持多种时间轴标记类型：

- `TimeMarker`: 单一时间点标记
- `TimeRangeMarker`: 时间范围标记

### 6.2 交互功能

- **缩放**: 使用鼠标滚轮进行时间轴缩放
- **平移**: 拖拽进行时间轴平移
- **Y轴操作**: 按住 Ctrl 键进行Y轴平移和缩放
- **数据点高亮**: 悬停显示具体数值

### 6.3 性能优化

IOTWave 内置了多项性能优化技术：

1. **智能数据下采样**: 对大量数据点进行下采样，保持视觉效果的同时提高性能
2. **边界点保留**: 下采样时保留边界点，确保不丢失关键数据
3. **极值保留算法**: 在每个像素区间内保留极值（最大值和最小值）
4. **缓存机制**: 缓存画刷和几何图形，减少重复创建开销

## 7. 示例代码

### 7.1 完整示例

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IOTWave.Views;
using IOTWave.Models;
using Avalonia.Media;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // 创建波形面板
        var wavePanel = new WaveListPanel
        {
            StartTime = DateTime.Now.AddHours(-1),
            EndTime = DateTime.Now,
            AutoDistributePanelHeight = true
        };

        // 创建温度曲线
        var tempCurve = new CurveData
        {
            Name = "Temperature",
            Color = Colors.Red,
            ShowPoints = true,
            LineWidth = 2
        };

        // 生成示例数据
        var baseTime = DateTime.Now.AddHours(-1);
        for (int i = 0; i < 3600; i++) // 1小时数据，每秒一个点
        {
            tempCurve.Points.Add(new TimePoint
            {
                Time = baseTime.AddSeconds(i),
                Value = 20 + 5 * Math.Sin(i * 0.01) + Random.Shared.NextDouble() * 2
            });
        }

        // 创建曲线组
        var tempGroup = new CurveGroup
        {
            Name = "Temperature Sensor",
            Curves = new List<CurveData> { tempCurve },
            YMarkers = new List<YMarker>
            {
                new YMarker(25, "Upper Limit"),
                new YMarker(15, "Lower Limit")
            }
        };

        // 设置数据源
        wavePanel.ItemsSource = new List<object> { tempGroup };

        // 设置为主内容
        Content = wavePanel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
```

### 7.2 多曲线显示

```csharp
// 创建多个曲线
var temperatureCurve = new CurveData
{
    Name = "Temperature",
    Color = Colors.Red,
    Points = GenerateTemperatureData()
};

var pressureCurve = new CurveData
{
    Name = "Pressure",
    Color = Colors.Blue,
    Points = GeneratePressureData()
};

var humidityCurve = new CurveData
{
    Name = "Humidity",
    Color = Colors.Green,
    Points = GenerateHumidityData()
};

// 创建曲线组
var sensorGroup = new CurveGroup
{
    Name = "Environment Sensors",
    Curves = new List<CurveData> { temperatureCurve, pressureCurve, humidityCurve }
};
```

## 8. 最佳实践

### 8.1 数据准备

- 使用 `TimePoint` 对象存储时间序列数据
- 按时间排序数据点以获得最佳性能
- 使用批量操作添加大量数据点

### 8.2 性能建议

- 对于大量数据，利用内置的下采样功能
- 合理设置 `PointShowLimit` 以控制数据点密度
- 分组相似的曲线到同一个 `CurveGroup`
- 避免过于频繁的UI更新

### 8.3 用户体验

- 提供清晰的图例和标记
- 使用对比鲜明的颜色区分不同曲线
- 合理设置Y轴标记以提供参考
- 使用相对时间模式简化时间理解

## 9. 故障排除

### 9.1 常见问题

**Q: 数据没有显示？**
A: 检查时间范围设置，确保 `StartTime` 和 `EndTime` 涵盖数据时间范围

**Q: 性能低下？**
A: 检查数据点数量，考虑使用下采样或分页加载

**Q: 颜色没有应用？**
A: 确保设置了 `Color` 属性而不是其他颜色相关属性

### 9.2 调试技巧

- 使用 `MinValue` 和 `MaxValue` 属性验证数据范围
- 检查 `IsVisible` 属性确认元素可见性
- 使用 `YMarkers` 验证Y轴刻度设置

## 10. API 参考

### 10.1 IChartGlobal 接口

实现该接口可自定义图表全局配置：

```csharp
public interface IChartGlobal
{
    DateTime StartTime { get; }
    DateTime EndTime { get; }
    bool UseRelativeTime { get; }
    DateTime RelativeTimeBase { get; }
    string RelativeTimeBaseLabel { get; }
    double TimeToX(DateTime time);
    DateTime XToTime(double x);
    // ... 其他成员
}
```

## 11. 贡献指南

欢迎贡献！请遵循以下步骤：

1. Fork 项目
2. 创建功能分支
3. 提交更改
4. 发起 Pull Request

## 12. 许可证

本项目采用 LGPL-2.1-or-later 许可证。

## 13. 支持

- 邮箱: jionfull@163.com
- GitHub Issues: https://github.com/jionfull/IOTWave/issues
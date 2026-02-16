# IOTWave Quick Start Guide

## 安装

通过 NuGet 安装：

```bash
dotnet add package IOTWave
```

## 基本使用

### 1. 添加样式引用

在 `App.axaml` 中添加 IOTWave 样式：

```xml
<Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://IOTWave/Themes.axaml"/>
</Application.Styles>
```

### 2. 创建 ViewModel

创建一个继承自 `IOTWaveBaseViewModel` 的视图模型：

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using IotWave.ViewModels;
using IotWave.Models;
using Avalonia.Media;
using System.Collections.ObjectModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private IOTWaveBaseViewModel viewModel = new IOTWaveBaseViewModel();

    public MainViewModel()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        var startTime = DateTime.Now.Date;
        ViewModel.BeginTime = startTime;
        ViewModel.EndTime = startTime.AddHours(24);

        // 创建曲线面板
        var curveGroup = new CurveGroup
        {
            Legend = "温度监控",
            Height = 200
        };

        // 添加曲线数据
        var curve = new CurveData
        {
            Name = "温度传感器",
            Color = Color.Parse("#FF0000")
        };

        // 添加数据点
        for (int i = 0; i < 100; i++)
        {
            curve.Points.Add(new TimePoint
            {
                Time = startTime.AddSeconds(i),
                Value = 20 + Math.Sin(i * 0.1) * 10
            });
        }

        curveGroup.Curves.Add(curve);
        ViewModel.Items.Add(curveGroup);
    }
}
```

### 3. 在视图中使用控件

在 XAML 中使用 `IOTChartBase` 控件：

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:iot="using:IOTWave"
             x:Class="YourApp.Views.MainView">
    
    <iot:IOTChartBase DataContext="{Binding ViewModel}" />
    
</UserControl>
```

## 核心概念

### 数据模型

#### TimePoint - 时间数据点

```csharp
var point = new TimePoint
{
    Time = DateTime.Now,
    Value = 25.5
};
```

#### CurveData - 曲线数据

```csharp
var curve = new CurveData
{
    Name = "传感器1",
    Color = Colors.Red,
    IsVisible = true
};
curve.Points.Add(new TimePoint { Time = DateTime.Now, Value = 25.5 });
```

#### CurveGroup - 曲线组

```csharp
var curveGroup = new CurveGroup
{
    Legend = "温度监控",
    Height = 200
};
curveGroup.Curves.Add(curve1);
curveGroup.Curves.Add(curve2);

// 添加Y轴标记线
curveGroup.YMarkers.Add(new YMarker(20, "低温报警"));
curveGroup.YMarkers.Add(new YMarker(30, "高温警告"));
```

#### StatuSeriesGroup - 状态序列组

用于显示设备状态变化：

```csharp
var statusPanel = new StatuSeriesGroup
{
    Name = "设备状态",
    Height = 32
};

// 定义状态颜色
statusPanel.Brushes = new Dictionary<int, IBrush>
{
    { 0, new SolidColorBrush(Colors.Gray) },    // 停止
    { 1, new SolidColorBrush(Colors.Green) },   // 运行
    { 2, new SolidColorBrush(Colors.Orange) },  // 警告
    { 3, new SolidColorBrush(Colors.Red) }      // 故障
};

// 定义状态描述
statusPanel.Descriptions = new Dictionary<int, string>
{
    { 0, "停止" },
    { 1, "运行" },
    { 2, "警告" },
    { 3, "故障" }
};

// 添加状态变化点
statusPanel.Points.Add(new StatuPoint { Time = startTime, Value = 0 });
statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(10), Value = 1 });
statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(30), Value = 2 });
```

### ViewModel 属性

| 属性 | 类型 | 说明 |
|------|------|------|
| BeginTime | DateTime | 视图开始时间 |
| EndTime | DateTime | 视图结束时间 |
| SelectedTime | DateTime? | 当前选中的时间点 |
| Items | ObservableCollection<DataSeriesGroupBase> | 数据项集合 |

## 功能特性

### 交互操作

- **缩放**：鼠标滚轮或 Ctrl++/Ctrl+-
- **平移**：鼠标拖拽
- **光标**：点击定位时间点
- **重置Y轴**：Ctrl+R

### 自定义样式

控件支持以下样式属性：

- `GridBrush` - 网格线颜色
- `LabelBrush` - 标签颜色
- `ShowCursor` - 显示/隐藏光标
- `ShowTimeAxis` - 显示/隐藏时间轴
- `IsInteractive` - 启用/禁用交互

## 完整示例

```xml
<ioc:IOTChartBase DataContext="{Binding ViewModel}"
                  GridBrush="Gray"
                  LabelBrush="Aqua"
                  ShowCursor="True"
                  ShowTimeAxis="True"
                  IsInteractive="True" />
```

## 多目标框架支持

IOTWave 支持：
- .NET 8.0
- .NET 10.0

## 许可证

本项目采用 LGPL-2.1-or-later 许可证。

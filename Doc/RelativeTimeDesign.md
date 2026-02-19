# 相对时间刻度显示设计文档

## 1. 功能概述

### 1.1 背景

在工业物联网（IOT）波形图表应用中，经常需要分析事件型数据。例如，设备启动时刻为 9:05:43，持续 7 秒的数据采集，用户希望以启动时刻为基准，查看 0s、2s、4s、6s、8s 等相对时间刻度，而非绝对时间。

### 1.2 目标

- 提供相对时间显示模式，时间轴显示与基准时间的差值
- 时间刻度对齐到基准时间，确保刻度值为整数倍（如 2s、4s、6s）
- 光标、时间标记、时间范围标记同步显示相对时间
- 支持多种时间精度的格式化显示（毫秒、秒、分、时、天）

### 1.3 使用场景

| 场景 | 描述 |
|------|------|
| 事件分析 | 分析特定事件触发后的波形变化 |
| 故障诊断 | 从故障发生时刻开始计算时间差 |
| 实验数据 | 以实验开始时间为基准分析数据 |
| 设备启停 | 监控设备启动后的参数变化 |

---

## 2. 技术架构

### 2.1 整体架构图

```
┌─────────────────────────────────────────────────────────────────┐
│                         IOTWaveBaseViewModel                     │
│  ┌─────────────────────────────┐ ┌─────────────────────────────┐│
│  │ UseRelativeTime: bool       │ │ RelativeTimeBase: DateTime  ││
│  └─────────────────────────────┘ └─────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 ▼ Binding
┌─────────────────────────────────────────────────────────────────┐
│                           IOTChart                               │
│  ┌─────────────────────────────┐ ┌─────────────────────────────┐│
│  │ UseRelativeTime             │ │ RelativeTimeBase            ││
│  └─────────────────────────────┘ └─────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 ▼ TemplateBinding
┌─────────────────────────────────────────────────────────────────┐
│                         WaveListPanel                            │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │                    IChartGlobal 接口实现                   │   │
│  │  - UseRelativeTime: bool                                  │   │
│  │  - RelativeTimeBase: DateTime                             │   │
│  │  - CalculateFirstTick(): 刻度对齐算法                      │   │
│  └──────────────────────────────────────────────────────────┘   │
│                              │                                   │
│              ┌───────────────┼───────────────┐                  │
│              ▼               ▼               ▼                  │
│        ┌──────────┐   ┌──────────┐   ┌──────────────┐          │
│        │ TimeAxis │   │CursorCanvas│   │  CurvePanel │          │
│        └──────────┘   └──────────┘   └──────────────┘          │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 核心接口

```csharp
public interface IChartGlobal
{
    DateTime StartTime { get; }
    DateTime EndTime { get; }
    
    /// <summary>
    /// 是否使用相对时间模式
    /// </summary>
    bool UseRelativeTime { get; }
    
    /// <summary>
    /// 相对时间的基准时间点（显示为 0s）
    /// </summary>
    DateTime RelativeTimeBase { get; }
    
    double TimeToX(DateTime time);
    DateTime XToTime(double x);
    List<long> CalculateTickIntervals();
    // ... 其他成员
}
```

---

## 3. 核心组件设计

### 3.1 WaveListPanel - 刻度对齐算法

刻度对齐是相对时间显示的核心。算法确保刻度位置对齐到基准时间。

```csharp
private long CalculateFirstTick(long interval)
{
    // 相对时间模式：刻度对齐到基准时间
    var alignmentBase = UseRelativeTime ? RelativeTimeBase.Ticks : 0;
    
    long firstTick;
    if (UseRelativeTime && alignmentBase != 0)
    {
        // 相对时间模式：刻度对齐到基准时间
        // 确保 (tickTime - RelativeTimeBase) 是 interval 的整数倍
        var remainder = (StartTime.Ticks - alignmentBase) % interval;
        firstTick = StartTime.Ticks - remainder;
        if (remainder < 0)
        {
            firstTick -= interval;
        }
        if (firstTick < StartTime.Ticks)
        {
            firstTick += interval;
        }
    }
    else
    {
        // 普通模式：对齐到自然时间
        var remainder = StartTime.Ticks % interval;
        firstTick = StartTime.Ticks - remainder + interval;
        if (remainder > interval / 2)
        {
            firstTick += interval;
        }
    }

    return firstTick;
}
```

**算法说明：**

1. **基准对齐**：计算 `StartTime` 相对于 `RelativeTimeBase` 的余数
2. **首刻度计算**：从 `StartTime` 减去余数，得到第一个对齐刻度
3. **负数处理**：处理时间差为负数的情况
4. **边界修正**：确保首刻度不早于 `StartTime`

### 3.2 TimeAxis - 时间标签格式化

#### 3.2.1 相对时间格式化方法

```csharp
private string FormatRelativeTime(TimeSpan offset)
{
    if (offset.TotalSeconds < 0)
    {
        return $"-{FormatPositiveTime(offset.Duration())}";
    }
    return FormatPositiveTime(offset);
}

private static string FormatPositiveTime(TimeSpan duration)
{
    if (duration.TotalDays >= 1)
    {
        return $"{(int)duration.TotalDays}d {duration.Hours}:{duration.Minutes:D2}:{duration.Seconds:D2}";
    }
    else if (duration.TotalHours >= 1)
    {
        return $"{(int)duration.TotalHours}:{duration.Minutes:D2}:{duration.Seconds:D2}";
    }
    else if (duration.TotalMinutes >= 1)
    {
        return $"{(int)duration.TotalMinutes}:{duration.Seconds:D2}";
    }
    else if (duration.TotalSeconds >= 1)
    {
        return $"{duration.TotalSeconds:F1}s";
    }
    else
    {
        return $"{duration.TotalMilliseconds:F0}ms";
    }
}
```

#### 3.2.2 格式化规则

| 时间范围 | 格式示例 | 说明 |
|----------|----------|------|
| < 1秒 | `500ms` | 毫秒级显示 |
| 1秒 ~ 1分钟 | `3.5s` | 秒级显示，保留1位小数 |
| 1分钟 ~ 1小时 | `2:30` | 分:秒格式 |
| 1小时 ~ 1天 | `1:30:00` | 时:分:秒格式 |
| ≥ 1天 | `2d 3:30:00` | 天+时:分:秒格式 |
| 负时间 | `-2.5s` | 负号前缀 |

#### 3.2.3 起始标签显示

```csharp
private void DrawStartLabel(DrawingContext context)
{
    DateTime bTime = new DateTime(ChartGlobal.StartTime.Ticks);
    String labelText;
    String labelText2;
    
    if (ChartGlobal.UseRelativeTime)
    {
        // 相对时间模式
        var offset = bTime - ChartGlobal.RelativeTimeBase;
        labelText = FormatRelativeTime(offset);
        labelText2 = $"基准: {ChartGlobal.RelativeTimeBase:HH:mm:ss.fff}";
    }
    else
    {
        labelText = bTime.ToString("HH:mm:ss");
        labelText2 = bTime.ToString("M月d日");
    }
    // ... 绘制逻辑
}
```

### 3.3 CursorCanvas - 光标与标记显示

#### 3.3.1 光标时间显示

```csharp
private void DrawCursor(DrawingContext context, Rect bounds)
{
    // ... 绘制光标线
    
    if (CursorTime.HasValue && ChartGlobal != null)
    {
        string timeText;
        if (ChartGlobal.UseRelativeTime)
        {
            // 相对时间模式
            var offset = CursorTime.Value - ChartGlobal.RelativeTimeBase;
            timeText = FormatRelativeTime(offset);
        }
        else
        {
            timeText = CursorTime.Value.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        // ... 绘制文本
    }
}
```

#### 3.3.2 时间标记显示

```csharp
private void DrawTimeMarkers(DrawingContext context, Rect bounds)
{
    foreach (var marker in TimeMarkers)
    {
        // ... 绘制标记线
        
        if (!string.IsNullOrEmpty(marker.Label))
        {
            string displayLabel = marker.Label;
            if (ChartGlobal.UseRelativeTime)
            {
                var offset = marker.Time - ChartGlobal.RelativeTimeBase;
                displayLabel = $"{marker.Label} ({FormatRelativeTime(offset)})";
            }
            // ... 绘制标签
        }
    }
}
```

---

## 4. 数据绑定设计

### 4.1 属性定义

| 类 | 属性 | 类型 | 说明 |
|-----|------|------|------|
| WaveListPanel | UseRelativeTime | bool | 是否启用相对时间模式 |
| WaveListPanel | RelativeTimeBase | DateTime | 相对时间基准点 |
| IOTChart | UseRelativeTime | bool | 双向绑定到 WaveListPanel |
| IOTChart | RelativeTimeBase | DateTime | 双向绑定到 WaveListPanel |
| IOTWaveBaseViewModel | UseRelativeTime | bool | ViewModel 属性 |
| IOTWaveBaseViewModel | RelativeTimeBase | DateTime | ViewModel 属性 |

### 4.2 绑定路径

```
ViewModel.UseRelativeTime 
    └─Binding→ IOTChart.UseRelativeTime 
                  └─TemplateBinding→ WaveListPanel.UseRelativeTime

ViewModel.RelativeTimeBase 
    └─Binding→ IOTChart.RelativeTimeBase 
                  └─TemplateBinding→ WaveListPanel.RelativeTimeBase
```

---

## 5. 使用示例

### 5.1 基本使用

```csharp
// ViewModel 中设置
public partial class MainViewModel : IOTWaveBaseViewModel
{
    public void InitializeEventData()
    {
        // 设置基准时间为事件发生时刻
        RelativeTimeBase = new DateTime(2024, 1, 1, 9, 5, 43);
        
        // 启用相对时间模式
        UseRelativeTime = true;
        
        // 设置时间范围（事件前1秒到事件后8秒）
        StartTime = RelativeTimeBase.AddSeconds(-1);
        EndTime = RelativeTimeBase.AddSeconds(8);
    }
}
```

### 5.2 事件型曲线示例

```csharp
public void InitializeRelativeTimeData()
{
    // 事件时间：9:05:43
    var eventTime = new DateTime(2024, 1, 1, 9, 5, 43);
    
    // 配置相对时间模式
    RelativeTimeBase = eventTime;
    UseRelativeTime = true;
    
    // 数据参数：40点/秒，共7秒，共280个点
    int pointsPerSecond = 40;
    int durationSeconds = 7;
    int totalPoints = pointsPerSecond * durationSeconds;
    
    // 生成数据
    var startTime = eventTime;
    var timeStep = TimeSpan.FromSeconds(1.0 / pointsPerSecond);
    
    var dataPoints = new List<DataPoint>();
    for (int i = 0; i < totalPoints; i++)
    {
        var time = startTime + TimeSpan.FromTicks(timeStep.Ticks * i);
        var value = Math.Sin(i * 0.1) * 100 + 50;
        dataPoints.Add(new DataPoint(time, value));
    }
    
    // 设置时间范围
    StartTime = eventTime.AddSeconds(-0.5);
    EndTime = eventTime.AddSeconds(7.5);
}
```

### 5.3 XAML 绑定

```xml
<iot:IOTChart 
    UseRelativeTime="{Binding UseRelativeTime}"
    RelativeTimeBase="{Binding RelativeTimeBase}"
    StartTime="{Binding StartTime}"
    EndTime="{Binding EndTime}"
    ItemsSource="{Binding CurveGroups}" />
```

---

## 6. 显示效果说明

### 6.1 时间轴显示对比

**普通模式：**
```
9:05:42    9:05:44    9:05:46    9:05:48    9:05:50
```

**相对时间模式：**
```
-1s        2s         4s         6s         8s
基准: 09:05:43.000
```

### 6.2 光标显示对比

**普通模式：**
```
2024-01-01 09:05:45.500
```

**相对时间模式：**
```
2.5s
```

### 6.3 时间标记显示对比

**普通模式：**
```
峰值点
```

**相对时间模式：**
```
峰值点 (3.2s)
```

---

## 7. 性能考虑

### 7.1 计算优化

- 刻度对齐计算仅在时间范围变化时执行
- 时间格式化使用静态方法，减少对象创建
- 避免在渲染循环中创建新字符串

### 7.2 内存优化

- `TimeSpan` 计算使用 `Ticks` 级别整数运算
- 格式化字符串使用预定义格式

---

## 8. 扩展性设计

### 8.1 自定义时间格式化

可通过实现 `ITimeFormatter` 接口自定义时间格式：

```csharp
public interface ITimeFormatter
{
    string Format(TimeSpan duration);
    string FormatWithBase(TimeSpan duration, DateTime baseTime);
}
```

### 8.2 多基准时间支持

未来可扩展支持多个基准时间点：

```csharp
public class RelativeTimeConfig
{
    public DateTime PrimaryBase { get; set; }
    public Dictionary<string, DateTime> SecondaryBases { get; set; }
    public string ActiveBase { get; set; }
}
```

---

## 9. 版本历史

| 版本 | 日期 | 说明 |
|------|------|------|
| 1.0 | 2024-01 | 初始实现相对时间显示功能 |

---

## 10. 参考资料

- [Avalonia StyledProperty 文档](https://docs.avaloniaui.net/)
- [DateTime.Ticks 属性说明](https://learn.microsoft.com/dotnet/api/system.datetime.ticks)
- [TimeSpan 结构说明](https://learn.microsoft.com/dotnet/api/system.timespan)

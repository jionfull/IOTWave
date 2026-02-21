# 时间跳转功能设计文档

## 1. 功能概述

### 1.1 背景

在工业物联网（IOT）波形图表应用中，用户经常需要快速定位到数据的特定位置进行分析。例如：
- 查看数据起始时刻的波形
- 跳转到数据结束位置检查最新数据
- 定位到数据中间区域进行概览
- 跳转到特定时间点分析异常事件

### 1.2 目标

- 提供时间跳转功能，支持跳转到起始、结束、中间、指定时间
- 跳转时保持当前时间跨度（缩放比例）不变
- 确保时间轴和所有曲线同步跳转
- 避免事件循环，保证系统稳定性
- 支持 MVVM 模式，通过命令绑定实现

### 1.3 使用场景

| 场景 | 描述 |
|------|------|
| 快速定位 | 用户需要快速跳转到数据的起始或结束位置 |
| 异常分析 | 跳转到特定时间点分析异常事件 |
| 数据概览 | 跳转到数据中间区域进行整体概览 |
| 缩放导航 | 在放大查看细节后，跳转到其他位置保持相同的缩放级别 |

---

## 2. 技术架构

### 2.1 整体架构图

```
┌─────────────────────────────────────────────────────────────────┐
│                         IOTWaveBaseViewModel                     │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │ DataStartTime / DataEndTime: DateTime (数据范围)            ││
│  │ JumpTargetTime: DateTime (跳转目标时间)                      ││
│  │ JumpToStartCommand / JumpToEndCommand / ... (跳转命令)       ││
│  └─────────────────────────────────────────────────────────────┘│
│                              │                                   │
│                              ▼ TimeJumpRequested Event           │
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 ▼ Subscribe
┌─────────────────────────────────────────────────────────────────┐
│                           IOTChart2                              │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │ OnTimeJumpRequested() - 处理跳转事件                         ││
│  │ 调用 WaveListPanel 的跳转方法                                 ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 ▼ Call
┌─────────────────────────────────────────────────────────────────┐
│                         WaveListPanel                            │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ JumpToTime(DateTime) - 跳转到指定时间                      │   │
│  │ JumpToStart(DateTime) - 跳转到起始                         │   │
│  │ JumpToEnd(DateTime) - 跳转到结束                           │   │
│  │ JumpToMiddle(DateTime, DateTime) - 跳转到中间              │   │
│  │ _isJumping: bool - 防止事件循环标志                        │   │
│  └──────────────────────────────────────────────────────────┘   │
│                              │                                   │
│              ┌───────────────┼───────────────┐                  │
│              ▼               ▼               ▼                  │
│        ┌──────────┐   ┌──────────┐   ┌──────────────┐          │
│        │ TimeAxis │   │CursorCanvas│   │  CurvePanel │          │
│        │  更新刻度 │   │  更新光标  │   │  更新曲线   │          │
│        └──────────┘   └──────────┘   └──────────────┘          │
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 核心接口和类

#### TimeJumpType 枚举

```csharp
public enum TimeJumpType
{
    /// <summary>
    /// 跳转到起始
    /// </summary>
    Start,
    /// <summary>
    /// 跳转到结束
    /// </summary>
    End,
    /// <summary>
    /// 跳转到中间
    /// </summary>
    Middle,
    /// <summary>
    /// 跳转到指定时间
    /// </summary>
    SpecificTime
}
```

#### TimeJumpEventArgs 事件参数

```csharp
public class TimeJumpEventArgs
{
    public TimeJumpType JumpType { get; }
    public DateTime? TargetTime { get; }

    public TimeJumpEventArgs(TimeJumpType jumpType, DateTime? targetTime = null)
    {
        JumpType = jumpType;
        TargetTime = targetTime;
    }
}
```

---

## 3. 核心组件设计

### 3.1 WaveListPanel - 时间跳转方法

#### 3.1.1 跳转到指定时间（保持时间跨度）

```csharp
/// <summary>
/// 标志位，用于防止事件循环
/// </summary>
private bool _isJumping = false;

/// <summary>
/// 跳转到指定时间，保持当前时间跨度不变
/// </summary>
/// <param name="targetTime">目标中心时间</param>
public void JumpToTime(DateTime targetTime)
{
    if (_isJumping) return;

    try
    {
        _isJumping = true;
        var timeSpan = EndTime - StartTime;

        // 以目标时间为中心，保持时间跨度不变
        StartTime = targetTime - TimeSpan.FromTicks(timeSpan.Ticks / 2);
        EndTime = targetTime + TimeSpan.FromTicks(timeSpan.Ticks / 2);

        OnTimeRangeChanged();
    }
    finally
    {
        _isJumping = false;
    }
}
```

**算法说明：**

1. **事件循环防护**：使用 `_isJumping` 标志位，防止属性变化触发重复跳转
2. **时间跨度保持**：计算当前 `EndTime - StartTime` 并保持不变
3. **中心对齐**：以目标时间为新的可视范围中心
4. **事件通知**：调用 `OnTimeRangeChanged()` 通知所有相关组件更新

#### 3.1.2 跳转到起始位置

```csharp
/// <summary>
/// 跳转到数据起始位置
/// </summary>
/// <param name="dataStartTime">数据起始时间</param>
public void JumpToStart(DateTime dataStartTime)
{
    var timeSpan = EndTime - StartTime;
    StartTime = dataStartTime;
    EndTime = dataStartTime + timeSpan;
    OnTimeRangeChanged();
}
```

#### 3.1.3 跳转到结束位置

```csharp
/// <summary>
/// 跳转到数据结束位置
/// </summary>
/// <param name="dataEndTime">数据结束时间</param>
public void JumpToEnd(DateTime dataEndTime)
{
    var timeSpan = EndTime - StartTime;
    EndTime = dataEndTime;
    StartTime = dataEndTime - timeSpan;
    OnTimeRangeChanged();
}
```

#### 3.1.4 跳转到中间位置

```csharp
/// <summary>
/// 跳转到数据中间位置
/// </summary>
/// <param name="dataStartTime">数据起始时间</param>
/// <param name="dataEndTime">数据结束时间</param>
public void JumpToMiddle(DateTime dataStartTime, DateTime dataEndTime)
{
    var middleTime = dataStartTime + TimeSpan.FromTicks((dataEndTime - dataStartTime).Ticks / 2);
    JumpToTime(middleTime);
}
```

### 3.2 IOTWaveBaseViewModel - 跳转命令

#### 3.2.1 属性定义

```csharp
/// <summary>
/// 数据起始时间，用于时间跳转
/// </summary>
[ObservableProperty]
private DateTime _dataStartTime = DateTime.Now.AddDays(-1);

/// <summary>
/// 数据结束时间，用于时间跳转
/// </summary>
[ObservableProperty]
private DateTime _dataEndTime = DateTime.Now.AddDays(1);

/// <summary>
/// 指定跳转时间（用于跳转到指定时间功能）
/// </summary>
[ObservableProperty]
private DateTime _jumpTargetTime = DateTime.Now;
```

#### 3.2.2 命令定义

```csharp
/// <summary>
/// 时间跳转事件，用于通知 View 层执行跳转
/// </summary>
public event Action<TimeJumpEventArgs>? TimeJumpRequested;

/// <summary>
/// 跳转到起始位置命令
/// </summary>
[RelayCommand]
private void JumpToStart()
{
    TimeJumpRequested?.Invoke(new TimeJumpEventArgs(TimeJumpType.Start));
}

/// <summary>
/// 跳转到结束位置命令
/// </summary>
[RelayCommand]
private void JumpToEnd()
{
    TimeJumpRequested?.Invoke(new TimeJumpEventArgs(TimeJumpType.End));
}

/// <summary>
/// 跳转到中间位置命令
/// </summary>
[RelayCommand]
private void JumpToMiddle()
{
    TimeJumpRequested?.Invoke(new TimeJumpEventArgs(TimeJumpType.Middle));
}

/// <summary>
/// 跳转到指定时间命令
/// </summary>
[RelayCommand]
private void JumpToTargetTime()
{
    TimeJumpRequested?.Invoke(new TimeJumpEventArgs(TimeJumpType.SpecificTime, JumpTargetTime));
}
```

### 3.3 IOTChart2 - 事件订阅与处理

```csharp
private IOTWaveBaseViewModel? _currentViewModel;

protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
{
    base.OnApplyTemplate(e);
    _waveListPanel = e.NameScope.Find<WaveListPanel>("CurveDisplay");
    
    // 订阅 DataContext 变化
    this.GetObservable(DataContextProperty).Subscribe(OnDataContextChanged);
}

private void OnDataContextChanged(object? dataContext)
{
    // 取消旧 ViewModel 的订阅
    if (_currentViewModel != null)
    {
        _currentViewModel.TimeJumpRequested -= OnTimeJumpRequested;
    }

    _currentViewModel = dataContext as IOTWaveBaseViewModel;

    // 订阅新 ViewModel 的事件
    if (_currentViewModel != null)
    {
        _currentViewModel.TimeJumpRequested += OnTimeJumpRequested;
    }
}

private void OnTimeJumpRequested(TimeJumpEventArgs e)
{
    if (_waveListPanel == null || _currentViewModel == null) return;

    switch (e.JumpType)
    {
        case TimeJumpType.Start:
            _waveListPanel.JumpToStart(_currentViewModel.DataStartTime);
            break;
        case TimeJumpType.End:
            _waveListPanel.JumpToEnd(_currentViewModel.DataEndTime);
            break;
        case TimeJumpType.Middle:
            _waveListPanel.JumpToMiddle(_currentViewModel.DataStartTime, _currentViewModel.DataEndTime);
            break;
        case TimeJumpType.SpecificTime:
            if (e.TargetTime.HasValue)
            {
                _waveListPanel.JumpToTime(e.TargetTime.Value);
            }
            break;
    }
}
```

---

## 4. 数据流设计

### 4.1 跳转流程图

```
用户点击按钮
     │
     ▼
┌─────────────────────┐
│ Command.Execute()   │
│ (ViewModel)         │
└─────────────────────┘
     │
     ▼
┌─────────────────────┐
│ TimeJumpRequested   │
│ Event Invoke        │
└─────────────────────┘
     │
     ▼
┌─────────────────────┐
│ IOTChart2           │
│ OnTimeJumpRequested │
└─────────────────────┘
     │
     ▼
┌─────────────────────┐
│ WaveListPanel       │
│ JumpToTime()        │
└─────────────────────┘
     │
     ├──────────────────────┐
     │                      │
     ▼                      ▼
┌──────────┐        ┌──────────────┐
│StartTime │        │   EndTime    │
│  更新    │        │    更新      │
└──────────┘        └──────────────┘
     │                      │
     └──────────┬───────────┘
                │
                ▼
┌─────────────────────┐
│ OnTimeRangeChanged  │
└─────────────────────┘
                │
    ┌───────────┼───────────┐
    │           │           │
    ▼           ▼           ▼
┌───────┐ ┌──────────┐ ┌──────────┐
│TimeAxis│ │CursorCanvas│ │CurvePanel│
│ 更新   │ │   更新    │ │  更新    │
└───────┘ └──────────┘ └──────────┘
```

### 4.2 防止事件循环机制

```
┌─────────────────────────────────────────────────────────────┐
│                      事件循环场景                            │
│                                                              │
│  StartTime 变化 ──▶ 属性变更通知 ──▶ ViewModel 更新          │
│       ▲                              │                       │
│       │                              ▼                       │
│       └──── 触发新的跳转 ◀──── Binding 更新                  │
│                                                              │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                      解决方案                                │
│                                                              │
│  JumpToTime() 入口                                           │
│       │                                                      │
│       ▼                                                      │
│  if (_isJumping) return;  ◀─── 如果正在跳转，直接返回        │
│       │                                                      │
│       ▼                                                      │
│  _isJumping = true;  ◀─── 设置标志位                         │
│       │                                                      │
│       ▼                                                      │
│  执行跳转逻辑                                                 │
│       │                                                      │
│       ▼                                                      │
│  _isJumping = false;  ◀─── 清除标志位                        │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 5. 使用示例

### 5.1 XAML 绑定

```xml
<!-- 跳转按钮 -->
<StackPanel Orientation="Horizontal" Spacing="10">
    <Button Content="跳转到起始" 
            Command="{Binding TimeJumpViewModel.JumpToStartCommand}"/>
    
    <Button Content="跳转到中间" 
            Command="{Binding TimeJumpViewModel.JumpToMiddleCommand}"/>
    
    <Button Content="跳转到结束" 
            Command="{Binding TimeJumpViewModel.JumpToEndCommand}"/>
    
    <DatePicker SelectedDate="{Binding TimeJumpViewModel.JumpTargetTime}"/>
    <TimePicker SelectedTime="{Binding TimeJumpViewModel.JumpTargetTime}"/>
    
    <Button Content="跳转到指定时间" 
            Command="{Binding TimeJumpViewModel.JumpToTargetTimeCommand}"/>
</StackPanel>

<!-- 图表控件 -->
<views:IOTChart2 DataContext="{Binding TimeJumpViewModel}"/>
```

### 5.2 ViewModel 初始化

```csharp
public void InitializeTimeJumpData()
{
    var startTime = DateTime.Now.Date;
    var endTime = startTime.AddHours(24);

    // 设置数据范围
    TimeJumpViewModel.DataStartTime = startTime;
    TimeJumpViewModel.DataEndTime = endTime;

    // 设置初始可视范围
    TimeJumpViewModel.BeginTime = startTime;
    TimeJumpViewModel.EndTime = startTime.AddHours(4);

    // 设置默认跳转目标时间
    TimeJumpViewModel.JumpTargetTime = startTime.AddHours(12);

    // 添加数据...
}
```

### 5.3 程序化跳转

```csharp
// 直接调用 WaveListPanel 方法
waveListPanel.JumpToTime(new DateTime(2024, 1, 1, 12, 0, 0));
waveListPanel.JumpToStart(dataStartTime);
waveListPanel.JumpToEnd(dataEndTime);
waveListPanel.JumpToMiddle(dataStartTime, dataEndTime);

// 通过 ViewModel 命令
viewModel.JumpToStartCommand.Execute(null);
viewModel.JumpToTargetTimeCommand.Execute(null);
```

---

## 6. 单元测试

### 6.1 测试覆盖范围

| 测试类 | 测试内容 |
|--------|----------|
| `JumpToTime_ShouldPreserveTimeSpan` | 验证跳转后时间跨度保持不变 |
| `JumpToStart_ShouldMoveToDataStart` | 验证跳转到起始位置正确 |
| `JumpToEnd_ShouldMoveToDataEnd` | 验证跳转到结束位置正确 |
| `JumpToMiddle_ShouldMoveToDataCenter` | 验证跳转到中间位置正确 |
| `JumpToTime_WithCustomSpan_ShouldUseProvidedSpan` | 验证自定义时间跨度跳转 |
| `ViewModel_JumpToStartCommand_ShouldRaiseEvent` | 验证命令触发事件 |
| `ViewModel_JumpToTargetTimeCommand_ShouldRaiseEventWithTargetTime` | 验证指定时间跳转 |
| `JumpToTime_ShouldPreventEventLoop` | 验证防止事件循环机制 |
| `JumpToTime_AfterZoom_ShouldPreserveZoomedSpan` | 验证缩放后跳转保持缩放比例 |

### 6.2 测试示例

```csharp
[Test]
public void JumpToTime_AfterZoom_ShouldPreserveZoomedSpan()
{
    // Arrange
    var panel = new WaveListPanel { Width = 800, Height = 600 };
    var startTime = new DateTime(2024, 1, 1, 0, 0, 0);
    var endTime = new DateTime(2024, 1, 1, 4, 0, 0); // 4小时

    panel.StartTime = startTime;
    panel.EndTime = endTime;

    panel.Measure(new Size(800, 600));
    panel.Arrange(new Rect(0, 0, 800, 600));

    // 模拟用户缩放 - 放大2倍（时间跨度减半）
    var zoomedSpan = TimeSpan.FromHours(2);
    panel.StartTime = startTime;
    panel.EndTime = startTime + zoomedSpan;

    // Act - 在缩放后跳转
    var targetTime = new DateTime(2024, 1, 1, 10, 0, 0);
    panel.JumpToTime(targetTime);

    // Assert - 时间跨度应该保持缩放后的值
    var newSpan = panel.EndTime - panel.StartTime;
    Assert.That(newSpan, Is.EqualTo(zoomedSpan));
}
```

---

## 7. 性能考虑

### 7.1 渲染优化

- 时间跳转仅修改 `StartTime` 和 `EndTime` 属性
- 通过 `OnTimeRangeChanged()` 统一触发重绘，避免多次渲染
- 曲线绘制使用二分查找快速定位可见数据点

### 7.2 内存优化

- 不创建额外的数据结构
- 使用现有的时间转换方法 `TimeToX` / `XToTime`
- 事件订阅使用弱引用模式（通过 Observable 订阅）

---

## 8. 扩展性设计

### 8.1 未来扩展方向

1. **动画跳转**：添加跳转动画效果
2. **历史记录**：记录跳转历史，支持前进/后退
3. **书签功能**：保存常用时间点，快速跳转
4. **智能跳转**：自动跳转到异常点或关键事件

### 8.2 自定义扩展

```csharp
// 扩展 TimeJumpType 枚举
public enum TimeJumpType
{
    Start,
    End,
    Middle,
    SpecificTime,
    NextEvent,      // 跳转到下一个事件
    PreviousEvent,  // 跳转到上一个事件
    Bookmark        // 跳转到书签
}

// 扩展方法
public void JumpToNextEvent(IEnumerable<DateTime> events)
{
    var nextEvent = events.FirstOrDefault(t => t > StartTime);
    if (nextEvent != default)
    {
        JumpToTime(nextEvent);
    }
}
```

---

## 9. 版本历史

| 版本 | 日期 | 说明 |
|------|------|------|
| 1.0 | 2024-02 | 初始实现时间跳转功能 |

---

## 10. 参考资料

- [Avalonia StyledProperty 文档](https://docs.avaloniaui.net/)
- [CommunityToolkit.Mvvm 文档](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [DateTime 结构说明](https://learn.microsoft.com/dotnet/api/system.datetime)

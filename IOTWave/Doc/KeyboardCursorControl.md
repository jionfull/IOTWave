# 键盘控制光标移动功能

## 功能概述

IOTWave 支持通过键盘方向键控制时间光标（TimeCursor）的移动，适用于精确浏览波形数据的场景。

## 默认行为

| 按键 | 功能 |
|------|------|
| ← | 向左移动一个步长 |
| → | 向右移动一个步长 |
| Alt + ← | 向左移动 10 个步长（10倍速） |
| Alt + → | 向右移动 10 个步长（10倍速） |

## 步长类型

通过 `CursorMoveStep` 属性设置移动步长：

| 枚举值 | 时间单位 | 适用场景 |
|--------|---------|----------|
| `Microsecond` | 1μs（微秒） | 高频采样数据、微秒级精度 |
| `Millisecond` | 1ms（毫秒） | 毫秒级采样数据 |
| `Second` | 1s（秒） | 常规波形数据（默认值） |
| `Minute` | 1min（分钟） | 分钟级数据 |
| `Hour` | 1h（小时） | 小时级数据 |
| `Day` | 1d（天） | 日报表、长期趋势 |

## 使用示例

### WaveListPanel 直接使用

```xml
<views:WaveListPanel
    StartTime="{Binding BeginTime}"
    EndTime="{Binding EndTime}"
    ItemsSource="{Binding Items}"
    CursorMoveStep="Second"
    EnableKeyboardCursorControl="True">
    <!-- 数据模板 -->
</views:WaveListPanel>
```

### IOTChart2 使用

```xml
<!-- 默认秒步长 -->
<views:IOTChart2 DataContext="{Binding ViewModel}" />

<!-- 日报表：天步长 -->
<views:IOTChart2 
    DataContext="{Binding DayReportViewModel}"
    CursorMoveStep="Day" />

<!-- 高频数据：毫秒步长 -->
<views:IOTChart2 
    DataContext="{Binding HighFreqViewModel}"
    CursorMoveStep="Millisecond" />

<!-- 禁用键盘控制 -->
<views:IOTChart2 
    DataContext="{Binding ViewModel}"
    EnableKeyboardCursorControl="False" />
```

### 代码中设置

```csharp
// 设置步长为毫秒
waveListPanel.CursorMoveStep = CursorMoveStep.Millisecond;

// 设置步长为天
iotChart2.CursorMoveStep = CursorMoveStep.Day;

// 禁用键盘控制
waveListPanel.EnableKeyboardCursorControl = false;
```

## 完整示例

### 日报表示例

```xml
<TabItem Header="日报表">
    <views:WaveListPanel
        Classes="demoPanel"
        DataContext="{Binding DayReportViewModel}"
        ItemsSource="{Binding Items}"
        StartTime="{Binding BeginTime}"
        EndTime="{Binding EndTime}"
        CursorMoveStep="Day"
        EnableKeyboardCursorControl="True">
        <views:WaveListPanel.DataTemplates>
            <DataTemplate DataType="models:CurveGroup">
                <views:CurvePanel
                    ChartGlobal="{Binding $parent[views:WaveListPanel]}"
                    Items="{Binding}"
                    Height="{Binding Height}" />
            </DataTemplate>
        </views:WaveListPanel.DataTemplates>
    </views:WaveListPanel>
</TabItem>
```

### 高频采样示例

```xml
<TabItem Header="高频采样">
    <views:IOTChart2
        DataContext="{Binding HighFreqViewModel}"
        CursorMoveStep="Microsecond" />
</TabItem>
```

## 属性说明

### CursorMoveStep

- **类型**：`CursorMoveStep` 枚举
- **默认值**：`CursorMoveStep.Second`
- **说明**：控制按键移动的时间单位

### EnableKeyboardCursorControl

- **类型**：`bool`
- **默认值**：`true`
- **说明**：是否启用键盘控制光标移动功能

## 注意事项

1. **焦点要求**：控件需要获得键盘焦点才能响应按键事件
2. **边界限制**：光标移动会自动限制在可见时间范围内
3. **Alt 键加速**：按住 Alt 键可将移动速度提升 10 倍
4. **性能考虑**：对于高频数据，建议使用较大的步长以提高浏览效率

## 相关主题

- [快速入门](QuickStart.md)
- [时间跳转功能](TimeJumpDesign.md)

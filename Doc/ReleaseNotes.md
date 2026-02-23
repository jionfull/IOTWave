# IOTWave v1.0.0 发布说明

## 版本信息
- 版本号: 1.0.0
- 发布日期: 2026年2月23日
- 支持的框架: .NET 8.0, .NET 10.0

## 功能特性

### 核心功能
- **高性能波形显示**: 基于Avalonia UI框架的高性能波形显示控件
- **多框架支持**: 同时支持.NET 8.0和.NET 10.0
- **时间轴功能**: 支持时间范围选择和缩放
- **交互操作**: 支持鼠标滚轮缩放、拖拽平移等交互功能

### 数据模型
- **CurveData**: 曲线数据模型，支持颜色、线条宽度、数据点等属性
- **CurveGroup**: 曲线组模型，可包含多条曲线及Y轴标记
- **TimePoint**: 时间点模型，包含时间和数值信息
- **YMarker**: Y轴标记，用于显示水平参考线

### 显示功能
- **自动高度分配**: WaveListPanel支持自动分配各面板高度
- **Y轴渲染**: 独立的Y轴渲染器，支持自定义刻度
- **标记系统**: 支持时间轴标记和Y轴标记
- **性能优化**: 包含数据点下采样算法，提升大数据量渲染性能

### 交互功能
- **时间轴缩放**: 鼠标滚轮实现时间轴缩放
- **Y轴缩放**: 按住Ctrl键+滚轮实现Y轴缩放
- **平移操作**: 鼠标拖拽实现X轴和Y轴平移
- **数据点高亮**: 悬停显示数据点详细信息

## 技术细节

### 依赖项
- Avalonia (11.3.12)
- CommunityToolkit.Mvvm (8.4.0)
- ReactiveUI (22.3.1)
- ReactiveUI.Avalonia (11.3.8)

### 性能优化
- 智能数据点下采样算法
- 空间复杂度优化的可见点筛选
- 极值保留的下采样算法
- UI元素缓存机制

## 使用示例

### 基本用法
```csharp
using IOTWave.Views;
using IOTWave.Models;

// 创建波形面板
var wavePanel = new WaveListPanel
{
    StartTime = DateTime.Now.AddHours(-1),
    EndTime = DateTime.Now,
    AutoDistributePanelHeight = true
};

// 创建曲线数据
var curveData = new CurveData
{
    Name = "Temperature",
    Color = Colors.Red,
    Points = GenerateTimeSeriesData()
};

// 创建曲线组
var curveGroup = new CurveGroup
{
    Name = "Sensor Data",
    Curves = new List<CurveData> { curveData }
};

// 设置数据源
wavePanel.ItemsSource = new List<object> { curveGroup };
```

## 已知问题
- CurveData中的PropertyChanged事件存在覆盖问题，建议使用ObservableObject基类的功能
- 部分属性存在CS0108警告（隐藏继承成员）

## 致谢
感谢所有为此项目做出贡献的开发者！

## 许可证
LGPL-2.1-or-later
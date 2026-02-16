---
name: 添加TimeMarker和TimeRangeMarker绘制及示例
overview: 在WaveListPanel上实现TimeMarker（垂直线）和TimeRangeMarker（垂直区域）的绘制，并在Demo中添加示例
todos:
  - id: implement-marker-render
    content: 在WaveListPanel.axaml.cs中实现TimeMarker和TimeRangeMarker的Render方法
    status: completed
  - id: bind-markers
    content: 在IOTChart.axaml中绑定TimeMarkers和TimeRangeMarkers属性
    status: completed
    dependencies:
      - implement-marker-render
  - id: add-example-data
    content: 在MainViewModel中添加TimeMarker和TimeRangeMarker示例数据
    status: completed
    dependencies:
      - bind-markers
---

## 需求说明

添加TimeMarker（垂直时刻标记线）和TimeRangeMarker（垂直时间范围区域）的绘制功能，并在示例中展示。

## 现有代码分析

- `TimeMarker` 模型已存在（IOTWave/Models/TimeMarker.cs）：包含Time、Label、Color、IsVisible属性
- `TimeRangeMarker` 模型已存在（IOTWave/Models/TimeRangeMarker.cs）：包含StartTime、EndTime、Label、Color、IsVisible属性
- `WaveListPanel` 已有TimeMarkers和TimeRangeMarkers属性（属性定义存在，但缺少渲染逻辑）
- `IOTChart.axaml` 已有WaveListPanel绑定，需添加TimeMarkers和TimeRangeMarkers绑定

## 核心功能

- 在WaveListPanel上渲染TimeMarker（垂直虚线+标签）
- 在WaveListPanel上渲染TimeRangeMarker（半透明垂直区域+标签）
- 在MainViewModel中添加示例数据

## 技术方案

- 在WaveListPanel.axaml.cs中添加Render方法override，绘制TimeMarker和TimeRangeMarker
- 使用DrawingContext进行绘制：TimeMarker绘制为垂直线，TimeRangeMarker绘制为半透明矩形区域
- 在IOTChart.axaml中将ViewModel的TimeMarkers/TimeRangeMarkers绑定到WaveListPanel
- 在MainViewModel中创建TimeMarker和TimeRangeMarker示例数据

## 实现细节

- WaveListPanel继承自SelectingItemsControl，需重写Render方法进行自定义绘制
- 使用ChartGlobal的TimeToX方法将时间转换为X坐标
- TimeMarker绘制：垂直线 + 标签文字
- TimeRangeMarker绘制：半透明填充矩形
- 确保绘制在所有面板内容之上（z-order）
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using IotWave.Models;
using IotWave.ViewModels;
using System;
using System.Collections.Generic;

namespace IOTWaveDemo.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private IOTWaveBaseViewModel basicViewModel = new IOTWaveBaseViewModel();

        [ObservableProperty]
        private IOTWaveBaseViewModel scrollableViewModel = new IOTWaveBaseViewModel();

        public MainViewModel()
        {
            InitializeBasicData();
            InitializeScrollableData();
        }

        private void InitializeBasicData()
        {
            // 创建模拟数据
            var rnd = new Random();
            var startTime = DateTime.Now.Date;
            BasicViewModel.BeginTime = startTime;
            BasicViewModel.EndTime = startTime.AddHours(24);
            
            // 状态面板
            var statusPanel = new StatuSeriesGroup
            {
                Name = "设备状态",
                Legend = "设备运行状态",
                Height = 32
            };

            // 定义状态值对应的颜色和描述
            statusPanel.Brushes = new Dictionary<int, IBrush>
        {
            { 0, new SolidColorBrush(Color.Parse("#CCCCCC")) }, // 停止 - 灰色
            { 1, new SolidColorBrush(Color.Parse("#00FF00")) }, // 运行 - 绿色
            { 2, new SolidColorBrush(Color.Parse("#FFA500")) }, // 警告 - 橙色
            { 3, new SolidColorBrush(Color.Parse("#FF0000") ) }  // 故障 - 红色
        };

            statusPanel.Descriptions = new Dictionary<int, string>
        {
            { 0, "停止" },
            { 1, "运行" },
            { 2, "警告" },
            { 3, "故障" }
        };

            // 生成状态变化点
            statusPanel.Points.Add(new StatuPoint { Time = startTime, Value = 0 });
            statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(10), Value = 1 });
            statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(45), Value = 2 });
            statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(60), Value = 1 });
            statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(120), Value = 3 });
            statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(135), Value = 1 });
            statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(180), Value = 2 });
            statusPanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(200), Value = 0 });

            BasicViewModel.Items.Add(statusPanel);

            // 设备运行模式状态面板 - 使用图像 Brush 示例
            var modePanel = new StatuSeriesGroup
            {
                Name = "运行模式",
                Legend = "设备运行模式（图像纹理）",
                Height = 32
            };

            // 定义运行模式值对应的图像 Brush 和描述
            modePanel.Brushes = new Dictionary<int, IBrush>
        {
            { 0, CreateImageBrush("double_green.png") },
            { 1, CreateImageBrush("double_yellow.png") },
            { 2, CreateImageBrush("green_yellow.png") },
            { 3, CreateImageBrush("yellow_flash_yellow.png") },
            { 4, CreateImageBrush("green_flash.png") },
            { 5, CreateImageBrush("guide_white.png") },
            { 6, CreateImageBrush("idle.png") },
            { 7, CreateImageBrush("locked.png") }
        };

            modePanel.Descriptions = new Dictionary<int, string>
        {
            { 0, "绿色双灯" },
            { 1, "黄色双灯" },
            { 2, "绿黄双灯" },
            { 3, "黄闪灯" },
            { 4, "绿闪灯" },
            { 5, "白色引导灯" },
            { 6, "空闲灯" },
            { 7, "锁定灯" }
        };

            // 生成运行模式变化点
            modePanel.Points.Add(new StatuPoint { Time = startTime, Value = 0 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(15), Value = 1 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(30), Value = 2 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(45), Value = 3 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(60), Value = 4 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(75), Value = 5 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(90), Value = 6 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(105), Value = 7 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(120), Value = 0 });

            BasicViewModel.Items.Add(modePanel);

            // 温度曲线数据
            var tempPanel = new CurveGroup()
            {
                Legend = "温度监控",
                Height = 200
            };

            var tempCurve1 = new CurveData
            {
                Name = "温度传感器1",
                Color = Color.Parse("#FF0000")
            };

            var tempCurve2 = new CurveData
            {
                Name = "温度传感器2",
                Color = Color.Parse("#0000FF")
            };

            // 生成模拟数据点
            for (int i = 0; i < 3600 * 24 * 10; i++)
            {
                var time = startTime.AddSeconds(i * 0.1);
                tempCurve1.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 20 + 10 * Math.Sin(i * Math.PI / 30) + rnd.NextDouble() * 2
                });

                tempCurve2.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 22 + 8 * Math.Cos(i * Math.PI / 25) + rnd.NextDouble() * 1.5
                });
            }

            tempPanel.Curves.Add(tempCurve1);
            tempPanel.Curves.Add(tempCurve2);

            // 添加 YMarkers
            tempPanel.YMarkers.Add(new YMarker(20, "低温报警"));
            tempPanel.YMarkers.Add(new YMarker(30, "高温警告"));
            tempPanel.YMarkers.Add(new YMarker(40, "高温报警"));

            BasicViewModel.Items.Add(tempPanel);

            // 湿度曲线数据
            var humidityPanel = new CurveGroup()
            {
                Legend = "湿度监控",
                Height = 150
            };

            var humidityCurve1 = new CurveData
            {
                Name = "湿度传感器1",
                Color = Color.Parse("#00AA00")
            };

            var humidityCurve2 = new CurveData
            {
                Name = "湿度传感器2",
                Color = Color.Parse("#00AAAA")
            };

            // 生成模拟数据点
            for (int i = 0; i < 3600 * 24 * 10; i++)
            {
                var time = startTime.AddSeconds(i * 0.1);
                humidityCurve1.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 50 + 20 * Math.Sin(i * Math.PI / 40) + rnd.NextDouble() * 5
                });

                humidityCurve2.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 55 + 15 * Math.Cos(i * Math.PI / 35) + rnd.NextDouble() * 4
                });
            }

            humidityPanel.Curves.Add(humidityCurve1);
            humidityPanel.Curves.Add(humidityCurve2);

            // 添加 YMarkers
            humidityPanel.YMarkers.Add(new YMarker(30, "低湿警告"));
            humidityPanel.YMarkers.Add(new YMarker(60, "高湿警告"));
            humidityPanel.YMarkers.Add(new YMarker(80, "高湿报警"));

            BasicViewModel.Items.Add(humidityPanel);

            // 添加TimeMarker示例
            BasicViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(6),
                Label = "早班开始",
                Color = Colors.Yellow
            });
            BasicViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(14),
                Label = "中班开始",
                Color = Colors.Orange
            });
            BasicViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(22),
                Label = "晚班开始",
                Color = Colors.Purple
            });

            // 添加TimeRangeMarker示例
            BasicViewModel.TimeRangeMarkers.Add(new TimeRangeMarker
            {
                StartTime = startTime.AddHours(8),
                EndTime = startTime.AddHours(12),
                Label = "上午高峰",
                Color = Color.FromArgb(60, 255, 255, 0)
            });
            BasicViewModel.TimeRangeMarkers.Add(new TimeRangeMarker
            {
                StartTime = startTime.AddHours(18),
                EndTime = startTime.AddHours(21),
                Label = "晚间高峰",
                Color = Color.FromArgb(60, 255, 100, 0)
            });
        }

        private void InitializeScrollableData()
        {
            // 创建模拟数据 - 多个曲线面板用于演示滚动条
            var rnd = new Random();
            var startTime = DateTime.Now.Date;
            ScrollableViewModel.BeginTime = startTime;
            ScrollableViewModel.EndTime = startTime.AddHours(24);

            // 创建多个曲线面板
            string[] sensorNames = { "温度", "湿度", "压力", "流量", "转速", "振动", "功率", "电压" };
            Color[] colors = { Colors.Red, Colors.Blue, Colors.Green, Colors.Orange, 
                              Colors.Purple, Colors.Cyan, Colors.Magenta, Colors.Brown };

            for (int panelIndex = 0; panelIndex < 8; panelIndex++)
            {
                var curvePanel = new CurveGroup()
                {
                    Legend = $"{sensorNames[panelIndex]}监控",
                    Height = 480
                };

                var curve = new CurveData
                {
                    Name = $"{sensorNames[panelIndex]}传感器",
                    Color = colors[panelIndex]
                };

                // 生成较少的数据点以加快加载
                for (int i = 0; i < 3600 * 24; i++)
                {
                    var time = startTime.AddSeconds(i * 0.1);
                    double baseValue = 50 + panelIndex * 10;
                    curve.Points.Add(new TimePoint
                    {
                        Time = time,
                        Value = baseValue + 20 * Math.Sin(i * Math.PI / (30 + panelIndex * 5)) + rnd.NextDouble() * 5
                    });
                }

                curvePanel.Curves.Add(curve);
                ScrollableViewModel.Items.Add(curvePanel);
            }

            // 添加TimeMarker示例
            ScrollableViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(8),
                Label = "工作开始",
                Color = Colors.Green
            });
            ScrollableViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(17),
                Label = "工作结束",
                Color = Colors.Red
            });
        }

        /// <summary>
        /// 创建图像 Brush
        /// </summary>
        private static ImageBrush CreateImageBrush(string assetPath)
        {
            var uri = new Uri($"avares://IotWaveDemo/Assets/Textures/{assetPath}");
            using var stream = AssetLoader.Open(uri);
            var bitmap = new Bitmap(stream);

            var brush = new ImageBrush(bitmap)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.Tile,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                DestinationRect = new RelativeRect(0, 0, bitmap.PixelSize.Height, bitmap.PixelSize.Width, RelativeUnit.Absolute)
            };
            return brush;
        }
    }
}

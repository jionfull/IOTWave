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
        private  IOTWaveBaseViewModel viewModel =new IOTWaveBaseViewModel();

        public MainViewModel()
        {
            InitializeData();
        }
        private void InitializeData()
        {
            // 创建模拟数据
            var rnd = new Random();
            var startTime = DateTime.Now.Date;
            ViewModel.BeginTime = startTime;
            ViewModel.EndTime = startTime.AddHours(24);

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

            ViewModel.Items.Add(tempPanel);

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

            ViewModel.Items.Add(humidityPanel);

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

            ViewModel.Items.Add(statusPanel);

            // 设备运行模式状态面板 - 使用图像 Brush 示例
            var modePanel = new StatuSeriesGroup
            {
                Name = "运行模式",
                Legend = "设备运行模式（图像纹理）",
                Height = 32
            };

            // 定义运行模式值对应的图像 Brush 和描述
            // 注意：图像 Brush 需要使用 ImageBrush,并从 Assets/Textures 加载图像
            modePanel.Brushes = new Dictionary<int, IBrush>
        {
            { 0, CreateImageBrush("double_green.png") },        // 绿色双灯
            { 1, CreateImageBrush("double_yellow.png") },       // 黄色双灯
            { 2, CreateImageBrush("green_yellow.png") },        // 绿黄双灯
            { 3, CreateImageBrush("yellow_flash_yellow.png") },  // 黄闪灯
            { 4, CreateImageBrush("green_flash.png") },          // 绿闪灯
            { 5, CreateImageBrush("guide_white.png") },          // 白色引导灯
            { 6, CreateImageBrush("idle.png") },                 // 空闲灯
            { 7, CreateImageBrush("locked.png") }                // 锁定灯
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

            // 生成运行模式变化点 - 展示多种灯的效果
            modePanel.Points.Add(new StatuPoint { Time = startTime, Value = 0 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(15), Value = 1 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(30), Value = 2 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(45), Value = 3 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(60), Value = 4 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(75), Value = 5 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(90), Value = 6 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(105), Value = 7 });
            modePanel.Points.Add(new StatuPoint { Time = startTime.AddMinutes(120), Value = 0 });



            ViewModel.Items.Add(modePanel);

            // 添加TimeMarker示例（垂直时刻标记线）
            ViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(6),
                Label = "早班开始",
                Color = Colors.Yellow
            });
            ViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(14),
                Label = "中班开始",
                Color = Colors.Orange
            });
            ViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = startTime.AddHours(22),
                Label = "晚班开始",
                Color = Colors.Purple
            });

            // 添加TimeRangeMarker示例（垂直时间范围区域）
            ViewModel.TimeRangeMarkers.Add(new TimeRangeMarker
            {
                StartTime = startTime.AddHours(8),
                EndTime = startTime.AddHours(12),
                Label = "上午高峰",
                Color = Color.FromArgb(60, 255, 255, 0)
            });
            ViewModel.TimeRangeMarkers.Add(new TimeRangeMarker
            {
                StartTime = startTime.AddHours(18),
                EndTime = startTime.AddHours(21),
                Label = "晚间高峰",
                Color = Color.FromArgb(60, 255, 100, 0)
            });
        }

      
        /// <summary>
        /// 创建图像 Brush
        /// </summary>
        /// <param name="assetPath">图像资源路径（相对于 Assets/Textures）</param>
        /// <returns>ImageBrush 实例</returns>
        private static ImageBrush CreateImageBrush(string assetPath)
        {
            var uri = new Uri($"avares://IotWaveDemo/Assets/Textures/{assetPath}");
            using var stream = AssetLoader.Open(uri);
            var bitmap = new Bitmap(stream);

            // 保存图片尺寸到 Tag 中，供绘制时使用
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

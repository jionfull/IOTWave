using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using IOTWave.Models;
using IOTWave.ViewModels;
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

        [ObservableProperty]
        private IOTWaveBaseViewModel relativeTimeViewModel = new IOTWaveBaseViewModel();

        [ObservableProperty]
        private IOTWaveBaseViewModel currentValueViewModel = new IOTWaveBaseViewModel();
       
        [ObservableProperty]
        private IOTWaveBaseViewModel dayReportViewModel = new IOTWaveBaseViewModel();


        public MainViewModel()
        {
            InitializeBasicData();
            InitializeScrollableData();
            InitializeRelativeTimeData();
            InitializeCurrentValueData();
            InitializeDayReportData();
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

        /// <summary>
        /// 初始化相对时间模式示例 - 事件型曲线数据
        /// 示例：启动时间为9点05分43秒，每秒40个点，共7秒数据
        /// </summary>
        private void InitializeRelativeTimeData()
        {
            var rnd = new Random();
            
            // 设定事件开始时间：9:05:43
            var eventStartTime = DateTime.Today.AddHours(9).AddMinutes(5).AddSeconds(43);
            
            // 设置相对时间基准为事件开始时间（显示为 0s）
            RelativeTimeViewModel.RelativeTimeBase = eventStartTime;
            RelativeTimeViewModel.UseRelativeTime = true;
            
            // 时间范围：事件开始前0.5秒到事件结束后0.5秒
            RelativeTimeViewModel.BeginTime = eventStartTime.AddSeconds(-0.5);
            RelativeTimeViewModel.EndTime = eventStartTime.AddSeconds(7.5);

            // 创建事件型曲线数据 - 模拟振动信号
            var vibrationPanel = new CurveGroup()
            {
                Legend = "振动信号",
                Height = 200
            };

            var vibrationCurve = new CurveData
            {
                Name = "振动传感器",
                Color = Color.Parse("#00FF00")
            };

            // 每秒40个点，共7秒数据 = 280个点
            int pointsPerSecond = 40;
            int totalSeconds = 7;
            int totalPoints = pointsPerSecond * totalSeconds;
            
            // 生成事件数据
            for (int i = 0; i < totalPoints; i++)
            {
                var time = eventStartTime.AddSeconds((double)i / pointsPerSecond);
                double value;
                
                // 模拟事件波形：前1秒平静，然后有突发信号，之后衰减
                double t = (double)i / pointsPerSecond;
                if (t < 1)
                {
                    // 静止阶段：小幅噪声
                    value = rnd.NextDouble() * 0.1 - 0.05;
                }
                else if (t < 2)
                {
                    // 突发阶段：大幅振动
                    value = Math.Sin((t - 1) * 50) * Math.Exp((t - 1) * 2) * 5;
                }
                else
                {
                    // 衰减阶段：指数衰减
                    value = Math.Sin((t - 1) * 50) * Math.Exp(-(t - 2) * 0.8) * 3;
                }
                
                vibrationCurve.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = value
                });
            }

            vibrationPanel.Curves.Add(vibrationCurve);
            vibrationPanel.YMarkers.Add(new YMarker(3, "警告阈值"));
            vibrationPanel.YMarkers.Add(new YMarker(-3, "警告阈值"));
            RelativeTimeViewModel.Items.Add(vibrationPanel);

            // 创建第二个曲线 - 模拟压力变化
            var pressurePanel = new CurveGroup()
            {
                Legend = "压力变化",
                Height = 150
            };

            var pressureCurve = new CurveData
            {
                Name = "压力传感器",
                Color = Color.Parse("#FF6B6B")
            };

            for (int i = 0; i < totalPoints; i++)
            {
                var time = eventStartTime.AddSeconds((double)i / pointsPerSecond);
                double t = (double)i / pointsPerSecond;
                double value;
                
                if (t < 1)
                {
                    value = 100;
                }
                else if (t < 3)
                {
                    value = 100 + (t - 1) * 20 + rnd.NextDouble() * 2;
                }
                else
                {
                    value = 140 - (t - 3) * 10 + rnd.NextDouble() * 2;
                }
                
                pressureCurve.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = value
                });
            }

            pressurePanel.Curves.Add(pressureCurve);
            pressurePanel.YMarkers.Add(new YMarker(120, "高压警告"));
            RelativeTimeViewModel.Items.Add(pressurePanel);

            // 添加时间标记 - 在相对时间模式下会显示相对时间
            RelativeTimeViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = eventStartTime,
                Label = "事件开始",
                Color = Colors.Cyan
            });
            RelativeTimeViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = eventStartTime.AddSeconds(2),
                Label = "峰值时刻",
                Color = Colors.Yellow
            });
            RelativeTimeViewModel.TimeMarkers.Add(new TimeMarker
            {
                Time = eventStartTime.AddSeconds(7),
                Label = "事件结束",
                Color = Colors.Orange
            });

            // 添加时间范围标记
            RelativeTimeViewModel.TimeRangeMarkers.Add(new TimeRangeMarker
            {
                StartTime = eventStartTime.AddSeconds(1),
                EndTime = eventStartTime.AddSeconds(2),
                Label = "突发阶段",
                Color = Color.FromArgb(80, 255, 100, 100)
            });
            RelativeTimeViewModel.TimeRangeMarkers.Add(new TimeRangeMarker
            {
                StartTime = eventStartTime.AddSeconds(2),
                EndTime = eventStartTime.AddSeconds(7),
                Label = "衰减阶段",
                Color = Color.FromArgb(60, 100, 200, 255)
            });
        }

        /// <summary>
        /// 初始化当前值显示示例 - 展示图例中显示光标位置的数值
        /// </summary>
        private void InitializeCurrentValueData()
        {
            var rnd = new Random();
            var startTime = DateTime.Now.Date;
            
            CurrentValueViewModel.BeginTime = startTime;
            CurrentValueViewModel.EndTime = startTime.AddHours(1);

            // 温度曲线面板
            var tempPanel = new CurveGroup()
            {
                Legend = "温度监控",
                Height = 200
            };

            var tempCurve1 = new CurveData
            {
                Name = "温度传感器1",
                Color = Color.Parse("#FF6B6B")
            };

            var tempCurve2 = new CurveData
            {
                Name = "温度传感器2",
                Color = Color.Parse("#4ECDC4")
            };

            // 生成数据点
            for (int i = 0; i < 3600; i++)
            {
                var time = startTime.AddSeconds(i);
                tempCurve1.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 25 + 5 * Math.Sin(i * Math.PI / 180) + rnd.NextDouble() * 1
                });
                tempCurve2.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 28 + 4 * Math.Cos(i * Math.PI / 150) + rnd.NextDouble() * 0.8
                });
            }

            tempPanel.Curves.Add(tempCurve1);
            tempPanel.Curves.Add(tempCurve2);
            tempPanel.YMarkers.Add(new YMarker(30, "高温警告"));
            CurrentValueViewModel.Items.Add(tempPanel);

            // 压力曲线面板
            var pressurePanel = new CurveGroup()
            {
                Legend = "压力监控",
                Height = 150
            };

            var pressureCurve = new CurveData
            {
                Name = "压力传感器",
                Color = Color.Parse("#45B7D1")
            };

            for (int i = 0; i < 3600; i++)
            {
                var time = startTime.AddSeconds(i);
                pressureCurve.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 100 + 10 * Math.Sin(i * Math.PI / 200) + rnd.NextDouble() * 2
                });
            }

            pressurePanel.Curves.Add(pressureCurve);
            pressurePanel.YMarkers.Add(new YMarker(110, "高压警告"));
            CurrentValueViewModel.Items.Add(pressurePanel);
        }



        /// <summary>
        /// 初始化当前值显示示例 - 展示图例中显示光标位置的数值
        /// </summary>
        private void InitializeDayReportData()
        {
            var rnd = new Random();
            var startTime = DateTime.Now.Date;

            DayReportViewModel.BeginTime = startTime;
            DayReportViewModel.EndTime = startTime.AddDays(100);

            // 温度曲线面板
            var tempPanel = new CurveGroup()
            {
                Legend = "环境温度",
                Height = 200
            };

            var tempCurve1 = new CurveData
            {
                Name = "最大值",
                Color = Color.Parse("#FF6B6B"),
                ShowPoints = true,
                PointShowLimit = 86400 * 2 // 每像素代表10天以内时显示圆点
            };

            var tempCurve2 = new CurveData
            {
                Name = "最小值",
                Color = Color.Parse("#4ECDC4"),
                ShowPoints = true,
                PointShowLimit = 86400 * 2
            };

            // 生成数据点
            for (int i = 0; i < 100; i++)
            {
                var time = startTime.AddDays(i).AddHours(i%10);
                tempCurve1.Points.Add(new ReportPoint
                {
                    ReportTime = time,
                    Value = 25 + 5 * Math.Sin(i * Math.PI / 180) + rnd.NextDouble() * 1
                });
                tempCurve2.Points.Add(new ReportPoint
                {
                    ReportTime = time,
                    Value = 28 + 4 * Math.Cos(i * Math.PI / 150) + rnd.NextDouble() * 0.8
                });
            }

            tempPanel.Curves.Add(tempCurve1);
            tempPanel.Curves.Add(tempCurve2);
            tempPanel.YMarkers.Add(new YMarker(30, "高温警告"));
            DayReportViewModel.Items.Add(tempPanel);

            // 压力曲线面板
            var pressurePanel = new CurveGroup()
            {
                Legend = "压力监控",
                Height = 150
            };

            var pressureCurve = new CurveData
            {
                Name = "平均值",
                Color = Color.Parse("#45B7D1"),
                ShowPoints = true,
                PointShowLimit = 86400 * 10
            };

            for (int i = 0; i < 100; i++)
            {
                var time = startTime.AddDays(i).AddHours(rnd.Next(0, 10));
                pressureCurve.Points.Add(new TimePoint
                {
                    Time = time,
                    Value = 100 + 10 * Math.Sin(i * Math.PI / 200) + rnd.NextDouble() * 2
                });
            }

            pressurePanel.Curves.Add(pressureCurve);
            pressurePanel.YMarkers.Add(new YMarker(110, "高压警告"));
            DayReportViewModel.Items.Add(pressurePanel);
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using IOTWave.Models;
using System.Collections.ObjectModel;

namespace IOTWave.ViewModels;

public partial class IOTWaveBaseViewModel:ViewModelBase
{
     [ObservableProperty]
     private DateTime _beginTime=DateTime.Now.AddDays(-1);
     
     [ObservableProperty]
     private DateTime _endTime=DateTime.Now.AddDays(1);

     [ObservableProperty]
     private DateTime? _selectedTime;

     [ObservableProperty]
     private bool _useRelativeTime = false;

     [ObservableProperty]
     private DateTime _relativeTimeBase = DateTime.Now;

     [ObservableProperty]
     private string _relativeTimeBaseLabel = "基准";

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
     /// 跳转目标时间（用于时间跳转功能）
     /// </summary>
     [ObservableProperty]
     private DateTime? _jumpTargetTime = DateTime.Now;

    public ObservableCollection<DataSeriesGroupBase> Items { get; } = new();

    public ObservableCollection<TimeMarker> TimeMarkers { get; } = new();

    public ObservableCollection<TimeRangeMarker> TimeRangeMarkers { get; } = new();
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IOTWave.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
     /// 指定跳转时间（用于跳转到指定时间功能）
     /// </summary>
     [ObservableProperty]
     private DateTime _jumpTargetTime = DateTime.Now;

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


    public ObservableCollection<DataSeriesGroupBase> Items { get; } = new()
    {

    };

    public ObservableCollection<TimeMarker> TimeMarkers { get; } = new();

    public ObservableCollection<TimeRangeMarker> TimeRangeMarkers { get; } = new();
}

/// <summary>
/// 时间跳转类型
/// </summary>
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

/// <summary>
/// 时间跳转事件参数
/// </summary>
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
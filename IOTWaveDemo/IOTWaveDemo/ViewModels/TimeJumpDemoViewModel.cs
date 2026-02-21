using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IOTWave.ViewModels;
using IOTWave.Views;
using System;

namespace IOTWaveDemo.ViewModels;

/// <summary>
/// 时间跳转演示专用 ViewModel
/// </summary>
public partial class TimeJumpDemoViewModel : ObservableObject
{
    private readonly IOTWaveBaseViewModel _dataViewModel;
    private IOTChart2? _chart;

    /// <summary>
    /// 数据 ViewModel
    /// </summary>
    public IOTWaveBaseViewModel DataViewModel => _dataViewModel;

    /// <summary>
    /// 指定跳转时间的时间部分（用于 TimePicker 绑定）
    /// </summary>
    [ObservableProperty]
    private TimeSpan _jumpTargetTimeSpan = TimeSpan.Zero;

    public TimeJumpDemoViewModel(IOTWaveBaseViewModel dataViewModel)
    {
        _dataViewModel = dataViewModel;
    }

    /// <summary>
    /// 设置关联的图表控件
    /// </summary>
    public void SetChart(IOTChart2? chart)
    {
        _chart = chart;
    }

   

    /// <summary>
    /// 跳转到起始位置
    /// </summary>
    [RelayCommand]
    private void JumpToStart()
    {
        _chart?.WaveListPanel?.JumpToStart(_dataViewModel.DataStartTime);
    }

    /// <summary>
    /// 跳转到结束位置
    /// </summary>
    [RelayCommand]
    private void JumpToEnd()
    {
        _chart?.WaveListPanel?.JumpToEnd(_dataViewModel.DataEndTime);
    }

    /// <summary>
    /// 跳转到中间位置
    /// </summary>
    [RelayCommand]
    private void JumpToMiddle()
    {
        _chart?.WaveListPanel?.JumpToMiddle(_dataViewModel.DataStartTime, _dataViewModel.DataEndTime);
    }

    /// <summary>
    /// 跳转到指定时间
    /// </summary>
    [RelayCommand]
    private void JumpToTargetTime6()
    {
        _chart?.WaveListPanel?.JumpToTime(_dataViewModel.DataStartTime.AddHours(6));
    }
}

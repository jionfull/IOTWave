using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IOTWave.ViewModels;
using IOTWave.Views;
using System;
using Avalonia.Threading;

namespace IOTWaveDemo.ViewModels;

public partial class TimeJumpDemoViewModel : ObservableObject
{
    private readonly IOTWaveBaseViewModel _dataViewModel;
    private IOTChart2? _chart;

    private DispatcherTimer? _playTimer;
    private DateTime _playStartTime;
    private double _playSpeed = 1.0;

    public IOTWaveBaseViewModel DataViewModel => _dataViewModel;

    [ObservableProperty]
    private TimeSpan _jumpTargetTimeSpan = TimeSpan.Zero;

    [ObservableProperty]
    private bool _isPlaying = false;

    private static readonly double[] SpeedValues = { 0.5, 1.0, 2.0, 4.0, 8.0 };

    [ObservableProperty]
    private int _playSpeedIndex = 1;

    public double PlaySpeed => SpeedValues[PlaySpeedIndex];

    public TimeJumpDemoViewModel(IOTWaveBaseViewModel dataViewModel)
    {
        _dataViewModel = dataViewModel;
    }

    public void SetChart(IOTChart2? chart)
    {
        _chart = chart;
    }

    [RelayCommand]
    private void JumpToStart()
    {
        _chart?.WaveListPanel?.JumpToStart(_dataViewModel.DataStartTime);
    }

    [RelayCommand]
    private void JumpToEnd()
    {
        _chart?.WaveListPanel?.JumpToEnd(_dataViewModel.DataEndTime);
    }

    [RelayCommand]
    private void JumpToMiddle()
    {
        _chart?.WaveListPanel?.JumpToMiddle(_dataViewModel.DataStartTime, _dataViewModel.DataEndTime);
    }

    [RelayCommand]
    private void JumpToTargetTime630()
    {
        var targetTime = _dataViewModel.DataStartTime.Date.AddHours(6).AddMinutes(30);
        _chart?.WaveListPanel?.JumpToTime(targetTime);
    }

    [RelayCommand]
    private void JumpToTargetTime1725()
    {
        var targetTime = _dataViewModel.DataStartTime.Date.AddHours(17).AddMinutes(25);
        _chart?.WaveListPanel?.JumpToTime(targetTime);
    }

    [RelayCommand]
    private void Play()
    {
        if (IsPlaying)
        {
            StopPlayback();
        }
        else
        {
            StartPlayback();
        }
    }

    private void StartPlayback()
    {
        if (_chart?.WaveListPanel == null) return;

        IsPlaying = true;
        _playStartTime = DateTime.Now;

        var waveListPanel = _chart.WaveListPanel;
        var currentStartTime = waveListPanel.StartTime;
        var currentEndTime = waveListPanel.EndTime;
        var timeSpan = currentEndTime - currentStartTime;

        _playTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };

        _playTimer.Tick += (s, e) =>
        {
            if (!IsPlaying || _chart?.WaveListPanel == null)
            {
                StopPlayback();
                return;
            }

            var currentTime = waveListPanel.CursorTime;
             currentTime =  currentTime?.AddSeconds(0.1 * PlaySpeed);
           
            if (currentTime > _dataViewModel.DataEndTime)
            {
                StopPlayback();
                return;
            }

            if (currentTime == null)
            {
                return;
            }

            waveListPanel.JumpToTime(currentTime.Value);

       
        };

        _playTimer.Start();
    }

    private void StopPlayback()
    {
        IsPlaying = false;
        _playTimer?.Stop();
        _playTimer = null;
    }
}

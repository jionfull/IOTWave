using CommunityToolkit.Mvvm.ComponentModel;
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


    public ObservableCollection<DataSeriesGroupBase> Items { get; } = new()
    {

    };

    public ObservableCollection<TimeMarker> TimeMarkers { get; } = new();

    public ObservableCollection<TimeRangeMarker> TimeRangeMarkers { get; } = new();
}
using CommunityToolkit.Mvvm.ComponentModel;
using IotWave.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace IotWave.ViewModels;

public partial class IOTWaveBaseViewModel:ViewModelBase
{
     [ObservableProperty]
     private DateTime _beginTime=DateTime.Now.AddDays(-1);
     
     [ObservableProperty]
     private DateTime _endTime=DateTime.Now.AddDays(1);

     [ObservableProperty]
     private DateTime? _selectedTime;


    public ObservableCollection<DataSeriesGroupBase> Items { get; } = new()
    {

    };

    public ObservableCollection<TimeMarker> TimeMarkers { get; } = new();

    public ObservableCollection<TimeRangeMarker> TimeRangeMarkers { get; } = new();
}
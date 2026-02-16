using CommunityToolkit.Mvvm.ComponentModel;

namespace IotWave.ViewModels;

public partial class TimeSerialGroup:ViewModelBase
{
   [ObservableProperty]
   private string? _name;
   
   
}
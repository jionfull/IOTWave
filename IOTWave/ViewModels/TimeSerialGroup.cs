using CommunityToolkit.Mvvm.ComponentModel;

namespace IOTWave.ViewModels;

public partial class TimeSerialGroup:ViewModelBase
{
   [ObservableProperty]
   private string? _name;
   
   
}
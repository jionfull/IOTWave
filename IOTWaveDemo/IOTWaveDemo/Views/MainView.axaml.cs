using Avalonia.Controls;
using IOTWaveDemo.ViewModels;
using IOTWave.Views;

namespace IOTWaveDemo.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            // 设置 TimeJumpDemoViewModel 的 chart 引用
            if (DataContext is MainViewModel mainViewModel)
            {
                var chart = this.FindControl<IOTChart2>("TimeJumpChart");
                mainViewModel.TimeJumpDemoViewModel.SetChart(chart);
            }
        }
    }
}
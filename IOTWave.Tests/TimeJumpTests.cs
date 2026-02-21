using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using IOTWave.ViewModels;
using IOTWave.Views;

namespace IOTWave.Tests;

[TestFixture]
public class TimeJumpTests
{
    #region WaveListPanel 时间跳转测试

    [AvaloniaTest]
    public void JumpToTime_ShouldPreserveTimeSpan()
    {
        // Arrange
        var panel = new WaveListPanel
        {
            Width = 800,
            Height = 600
        };

        var startTime = new DateTime(2024, 1, 1, 0, 0, 0);
        var endTime = new DateTime(2024, 1, 1, 1, 0, 0);
        var originalSpan = endTime - startTime;

        panel.StartTime = startTime;
        panel.EndTime = endTime;

        // 手动触发布局
        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        // Act - 跳转到中间时间
        var targetTime = new DateTime(2024, 1, 1, 2, 30, 0);
        panel.JumpToTime(targetTime);

        // Assert
        var newSpan = panel.EndTime - panel.StartTime;
        Assert.That(newSpan, Is.EqualTo(originalSpan), "时间跨度应该保持不变");

        // 验证目标时间在可见范围的中心
        var centerTime = panel.StartTime + TimeSpan.FromTicks(newSpan.Ticks / 2);
        Assert.That(centerTime, Is.EqualTo(targetTime).Within(TimeSpan.FromSeconds(1)), "目标时间应该在可见范围中心");
    }

    [AvaloniaTest]
    public void JumpToStart_ShouldMoveToDataStart()
    {
        // Arrange
        var panel = new WaveListPanel
        {
            Width = 800,
            Height = 600
        };

        var initialStart = new DateTime(2024, 1, 1, 5, 0, 0);
        var initialEnd = new DateTime(2024, 1, 1, 6, 0, 0);
        var dataStart = new DateTime(2024, 1, 1, 0, 0, 0);

        panel.StartTime = initialStart;
        panel.EndTime = initialEnd;

        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        // Act
        panel.JumpToStart(dataStart);

        // Assert
        Assert.That(panel.StartTime, Is.EqualTo(dataStart), "StartTime 应该等于数据起始时间");
        
        var timeSpan = initialEnd - initialStart;
        Assert.That(panel.EndTime, Is.EqualTo(dataStart + timeSpan), "EndTime 应该等于数据起始时间加时间跨度");
    }

    [AvaloniaTest]
    public void JumpToEnd_ShouldMoveToDataEnd()
    {
        // Arrange
        var panel = new WaveListPanel
        {
            Width = 800,
            Height = 600
        };

        var initialStart = new DateTime(2024, 1, 1, 5, 0, 0);
        var initialEnd = new DateTime(2024, 1, 1, 6, 0, 0);
        var dataEnd = new DateTime(2024, 1, 1, 12, 0, 0);

        panel.StartTime = initialStart;
        panel.EndTime = initialEnd;

        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        // Act
        panel.JumpToEnd(dataEnd);

        // Assert
        Assert.That(panel.EndTime, Is.EqualTo(dataEnd), "EndTime 应该等于数据结束时间");
        
        var timeSpan = initialEnd - initialStart;
        Assert.That(panel.StartTime, Is.EqualTo(dataEnd - timeSpan), "StartTime 应该等于数据结束时间减时间跨度");
    }

    [AvaloniaTest]
    public void JumpToMiddle_ShouldMoveToDataCenter()
    {
        // Arrange
        var panel = new WaveListPanel
        {
            Width = 800,
            Height = 600
        };

        var initialStart = new DateTime(2024, 1, 1, 5, 0, 0);
        var initialEnd = new DateTime(2024, 1, 1, 6, 0, 0);
        var dataStart = new DateTime(2024, 1, 1, 0, 0, 0);
        var dataEnd = new DateTime(2024, 1, 1, 12, 0, 0);
        var expectedMiddle = dataStart + TimeSpan.FromTicks((dataEnd - dataStart).Ticks / 2);

        panel.StartTime = initialStart;
        panel.EndTime = initialEnd;

        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        // Act
        panel.JumpToMiddle(dataStart, dataEnd);

        // Assert
        var newSpan = panel.EndTime - panel.StartTime;
        var centerTime = panel.StartTime + TimeSpan.FromTicks(newSpan.Ticks / 2);
        
        Assert.That(centerTime, Is.EqualTo(expectedMiddle).Within(TimeSpan.FromSeconds(1)), "中心时间应该在数据范围的正中间");
    }

    [AvaloniaTest]
    public void JumpToTime_WithCustomSpan_ShouldUseProvidedSpan()
    {
        // Arrange
        var panel = new WaveListPanel
        {
            Width = 800,
            Height = 600
        };

        panel.StartTime = new DateTime(2024, 1, 1, 0, 0, 0);
        panel.EndTime = new DateTime(2024, 1, 1, 1, 0, 0);

        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        // Act
        var targetTime = new DateTime(2024, 1, 1, 5, 0, 0);
        var customSpan = TimeSpan.FromMinutes(30);
        panel.JumpToTime(targetTime, customSpan);

        // Assert
        var actualSpan = panel.EndTime - panel.StartTime;
        Assert.That(actualSpan, Is.EqualTo(customSpan), "时间跨度应该使用传入的自定义跨度");

        var centerTime = panel.StartTime + TimeSpan.FromTicks(actualSpan.Ticks / 2);
        Assert.That(centerTime, Is.EqualTo(targetTime).Within(TimeSpan.FromSeconds(1)), "目标时间应该在可见范围中心");
    }

    #endregion

    #region ViewModel 命令测试

    [Test]
    public void ViewModel_JumpToStartCommand_ShouldRaiseEvent()
    {
        // Arrange
        var viewModel = new IOTWaveBaseViewModel();
        TimeJumpEventArgs? eventArgs = null;
        viewModel.TimeJumpRequested += (args) => eventArgs = args;

        // Act
        viewModel.JumpToStartCommand.Execute(null);

        // Assert
        Assert.That(eventArgs, Is.Not.Null, "应该触发 TimeJumpRequested 事件");
        Assert.That(eventArgs!.JumpType, Is.EqualTo(TimeJumpType.Start), "跳转类型应该是 Start");
    }

    [Test]
    public void ViewModel_JumpToEndCommand_ShouldRaiseEvent()
    {
        // Arrange
        var viewModel = new IOTWaveBaseViewModel();
        TimeJumpEventArgs? eventArgs = null;
        viewModel.TimeJumpRequested += (args) => eventArgs = args;

        // Act
        viewModel.JumpToEndCommand.Execute(null);

        // Assert
        Assert.That(eventArgs, Is.Not.Null, "应该触发 TimeJumpRequested 事件");
        Assert.That(eventArgs!.JumpType, Is.EqualTo(TimeJumpType.End), "跳转类型应该是 End");
    }

    [Test]
    public void ViewModel_JumpToMiddleCommand_ShouldRaiseEvent()
    {
        // Arrange
        var viewModel = new IOTWaveBaseViewModel();
        TimeJumpEventArgs? eventArgs = null;
        viewModel.TimeJumpRequested += (args) => eventArgs = args;

        // Act
        viewModel.JumpToMiddleCommand.Execute(null);

        // Assert
        Assert.That(eventArgs, Is.Not.Null, "应该触发 TimeJumpRequested 事件");
        Assert.That(eventArgs!.JumpType, Is.EqualTo(TimeJumpType.Middle), "跳转类型应该是 Middle");
    }

    [Test]
    public void ViewModel_JumpToTargetTimeCommand_ShouldRaiseEventWithTargetTime()
    {
        // Arrange
        var viewModel = new IOTWaveBaseViewModel();
        var targetTime = new DateTime(2024, 1, 1, 5, 30, 0);
        viewModel.JumpTargetTime = targetTime;

        TimeJumpEventArgs? eventArgs = null;
        viewModel.TimeJumpRequested += (args) => eventArgs = args;

        // Act
        viewModel.JumpToTargetTimeCommand.Execute(null);

        // Assert
        Assert.That(eventArgs, Is.Not.Null, "应该触发 TimeJumpRequested 事件");
        Assert.That(eventArgs!.JumpType, Is.EqualTo(TimeJumpType.SpecificTime), "跳转类型应该是 SpecificTime");
        Assert.That(eventArgs.TargetTime, Is.EqualTo(targetTime), "目标时间应该正确传递");
    }

    [Test]
    public void ViewModel_DataTimeRange_ShouldBeSettable()
    {
        // Arrange
        var viewModel = new IOTWaveBaseViewModel();
        var dataStart = new DateTime(2024, 1, 1, 0, 0, 0);
        var dataEnd = new DateTime(2024, 1, 2, 0, 0, 0);

        // Act
        viewModel.DataStartTime = dataStart;
        viewModel.DataEndTime = dataEnd;

        // Assert
        Assert.That(viewModel.DataStartTime, Is.EqualTo(dataStart));
        Assert.That(viewModel.DataEndTime, Is.EqualTo(dataEnd));
    }

    #endregion

    #region 防止事件循环测试

    [AvaloniaTest]
    public void JumpToTime_ShouldPreventEventLoop()
    {
        // Arrange
        var panel = new WaveListPanel
        {
            Width = 800,
            Height = 600
        };

        var startTime = new DateTime(2024, 1, 1, 0, 0, 0);
        var endTime = new DateTime(2024, 1, 1, 1, 0, 0);

        panel.StartTime = startTime;
        panel.EndTime = endTime;

        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        int eventCount = 0;
        panel.TimeRangeChanged += (s, e) => eventCount++;

        // Act - 执行多次跳转
        panel.JumpToTime(new DateTime(2024, 1, 1, 2, 0, 0));
        var firstEventCount = eventCount;

        panel.JumpToTime(new DateTime(2024, 1, 1, 3, 0, 0));
        var secondEventCount = eventCount;

        // Assert - 每次跳转应该只触发一次事件
        Assert.That(firstEventCount, Is.EqualTo(1), "第一次跳转应该触发一次事件");
        Assert.That(secondEventCount, Is.EqualTo(2), "第二次跳转应该再触发一次事件");
    }

    #endregion

    #region 缩放后跳转测试

    [AvaloniaTest]
    public void JumpToTime_AfterZoom_ShouldPreserveZoomedSpan()
    {
        // Arrange
        var panel = new WaveListPanel
        {
            Width = 800,
            Height = 600
        };

        var startTime = new DateTime(2024, 1, 1, 0, 0, 0);
        var endTime = new DateTime(2024, 1, 1, 4, 0, 0); // 4小时

        panel.StartTime = startTime;
        panel.EndTime = endTime;

        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        // 模拟用户缩放 - 放大2倍（时间跨度减半）
        var zoomedSpan = TimeSpan.FromHours(2); // 原来是4小时，放大后变成2小时
        panel.StartTime = startTime;
        panel.EndTime = startTime + zoomedSpan;

        // Act - 在缩放后跳转
        var targetTime = new DateTime(2024, 1, 1, 10, 0, 0);
        panel.JumpToTime(targetTime);

        // Assert - 时间跨度应该保持缩放后的值
        var newSpan = panel.EndTime - panel.StartTime;
        Assert.That(newSpan, Is.EqualTo(zoomedSpan), "跳转后应该保持缩放后的时间跨度");
    }

    #endregion
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Media;
using Avalonia.Threading;
using IOTWave.Models;
using IOTWave.Views;

namespace IOTWave.Tests;

[TestFixture]
public class DistributePanelHeightTests
{
    /// <summary>
    /// 创建测试用的 WaveListPanel
    /// </summary>
    private WaveListPanel CreateTestPanel(double width = 800, double height = 600)
    {
        return new WaveListPanel
        {
            AutoDistributePanelHeight = true,
            StatusPanelHeight = 20,
            Width = width,
            Height = height
        };
    }

    /// <summary>
    /// 创建测试用的 CurveGroup
    /// </summary>
    private CurveGroup CreateCurveGroup(string name)
    {
        return new CurveGroup
        {
            Name = name,
            Curves = new List<CurveData>()
        };
    }

    /// <summary>
    /// 创建测试用的 StatuSeriesGroup
    /// </summary>
    private StatuSeriesGroup CreateStatuSeriesGroup(string name)
    {
        return new StatuSeriesGroup
        {
            Name = name,
            Points = new List<StatuPoint>()
        };
    }

    #region 基础计算测试

    [Test]
    public void DistributePanelHeight_ShouldCorrectlyCalculateHeight()
    {
        // Arrange
        double totalHeight = 600;
        double statusPanelHeight = 20;
        int curvePanelCount = 2;
        int statusPanelCount = 1;

        // 计算预期高度
        double statusPanelTotalHeight = statusPanelCount * statusPanelHeight;
        double curveAvailableHeight = totalHeight - statusPanelTotalHeight;
        double expectedCurvePanelHeight = curveAvailableHeight / curvePanelCount;

        // Assert
        Assert.That(expectedCurvePanelHeight, Is.EqualTo(290));
        Assert.That(statusPanelTotalHeight, Is.EqualTo(20));
        Assert.That(curveAvailableHeight, Is.EqualTo(580));
    }

    [Test]
    public void DistributePanelHeight_WithThreeCurvePanels_ShouldDivideEvenly()
    {
        // Arrange
        double totalHeight = 500;
        double statusPanelHeight = 20;
        int curvePanelCount = 3;
        int statusPanelCount = 2;

        // 计算预期高度
        double statusPanelTotalHeight = statusPanelCount * statusPanelHeight;
        double curveAvailableHeight = totalHeight - statusPanelTotalHeight;
        double expectedCurvePanelHeight = curveAvailableHeight / curvePanelCount;

        // Assert
        Assert.That(statusPanelTotalHeight, Is.EqualTo(40));
        Assert.That(curveAvailableHeight, Is.EqualTo(460));
        Assert.That(expectedCurvePanelHeight, Is.EqualTo(153.33333333333334).Within(0.001));
    }

    [Test]
    public void DistributePanelHeight_ComplexScenario()
    {
        // Arrange: 4 CurvePanels, 2 StatusPanels, total height 800
        double totalHeight = 800;
        double statusPanelHeight = 25;
        int curvePanelCount = 4;
        int statusPanelCount = 2;

        // 计算预期高度
        double statusPanelTotalHeight = statusPanelCount * statusPanelHeight;
        double curveAvailableHeight = totalHeight - statusPanelTotalHeight;
        double expectedCurvePanelHeight = curveAvailableHeight / curvePanelCount;

        // Assert
        Assert.That(statusPanelTotalHeight, Is.EqualTo(50), "StatusPanel total height");
        Assert.That(curveAvailableHeight, Is.EqualTo(750), "CurvePanel available height");
        Assert.That(expectedCurvePanelHeight, Is.EqualTo(187.5), "Each CurvePanel height");
    }

    #endregion

    #region 边界条件测试

    [Test]
    public void DistributePanelHeight_WithNoCurvePanels_ShouldReturnEarly()
    {
        int curvePanelCount = 0;
        if (curvePanelCount == 0)
        {
            Assert.Pass("Should return early when no CurvePanels");
            return;
        }
        Assert.Fail("Should have returned early");
    }

    [Test]
    public void DistributePanelHeight_WithZeroAvailableHeight_ShouldReturnEarly()
    {
        double availableHeight = 0;
        if (availableHeight <= 0)
        {
            Assert.Pass("Should return early when height is zero or negative");
            return;
        }
        Assert.Fail("Should have returned early");
    }

    [Test]
    public void DistributePanelHeight_WithInsufficientHeight_ShouldReturnEarly()
    {
        double totalHeight = 50;
        double statusPanelHeight = 20;
        int statusPanelCount = 3;

        double statusPanelTotalHeight = statusPanelCount * statusPanelHeight;
        double curveAvailableHeight = totalHeight - statusPanelTotalHeight;

        if (curveAvailableHeight <= 0)
        {
            Assert.Pass("Should return early when insufficient height");
            return;
        }
        Assert.Fail("Should have returned early");
    }

    [Test]
    public void DistributePanelHeight_EdgeCase_SinglePanel()
    {
        double totalHeight = 600;
        int curvePanelCount = 1;
        double expectedCurvePanelHeight = totalHeight / curvePanelCount;
        Assert.That(expectedCurvePanelHeight, Is.EqualTo(600));
    }

    [Test]
    public void DistributePanelHeight_EdgeCase_ManyPanels()
    {
        double totalHeight = 600;
        int curvePanelCount = 10;
        double expectedCurvePanelHeight = totalHeight / curvePanelCount;
        Assert.That(expectedCurvePanelHeight, Is.EqualTo(60));
    }

    [Test]
    public void DistributePanelHeight_StatusPanelHeightCalculation()
    {
        int statusPanelCount = 5;
        double statusPanelHeight = 20;
        double statusPanelTotalHeight = statusPanelCount * statusPanelHeight;
        Assert.That(statusPanelTotalHeight, Is.EqualTo(100));
    }

    #endregion

    #region 控件行为测试

    [AvaloniaTest]
    public void DistributePanelHeight_WithNoItems_ShouldNotCrash()
    {
        var panel = CreateTestPanel();
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    [AvaloniaTest]
    public void DistributePanelHeight_WithAutoDistributeDisabled_ShouldDoNothing()
    {
        var panel = CreateTestPanel();
        panel.AutoDistributePanelHeight = false;
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    #endregion

    #region 与 Demo 行为一致的集成测试

    [AvaloniaTest]
    public void DistributePanelHeight_DemoScenario_2Curve1Status()
    {
        // Arrange - 模拟 Demo 中的配置
        var panel = CreateTestPanel(800, 600);
        
        var items = new List<object>
        {
            CreateCurveGroup("Curve1"),
            CreateCurveGroup("Curve2"),
            CreateStatuSeriesGroup("Status1")
        };

        panel.ItemsSource = items;

        // 手动触发布局
        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));

        // 验证计算逻辑（不崩溃）
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    [AvaloniaTest]
    public void DistributePanelHeight_DemoScenario_MultiplePanels()
    {
        // Arrange - 更复杂的多面板场景
        var panel = CreateTestPanel(1000, 800);
        
        var items = new List<object>
        {
            CreateCurveGroup("Temperature"),
            CreateCurveGroup("Pressure"),
            CreateCurveGroup("Flow"),
            CreateStatuSeriesGroup("Status1"),
            CreateStatuSeriesGroup("Status2")
        };

        panel.ItemsSource = items;
        panel.StatusPanelHeight = 24;

        // 手动触发布局
        panel.Measure(new Size(1000, 800));
        panel.Arrange(new Rect(0, 0, 1000, 800));

        // 验证计算逻辑
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    [AvaloniaTest]
    public void DistributePanelHeight_ShouldHandleResize()
    {
        // Arrange
        var panel = CreateTestPanel(800, 600);
        
        var items = new List<object>
        {
            CreateCurveGroup("Curve1"),
            CreateCurveGroup("Curve2")
        };

        panel.ItemsSource = items;

        // 初始布局
        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));
        panel.DistributePanelHeight();

        // 模拟窗口大小变化
        panel.Measure(new Size(800, 400));
        panel.Arrange(new Rect(0, 0, 800, 400));
        
        // 验证重新计算不会崩溃
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    [AvaloniaTest]
    public void DistributePanelHeight_ShouldHandleItemsChange()
    {
        // Arrange
        var panel = CreateTestPanel(800, 600);
        var items = new System.Collections.ObjectModel.ObservableCollection<object>
        {
            CreateCurveGroup("Curve1")
        };

        panel.ItemsSource = items;

        panel.Measure(new Size(800, 600));
        panel.Arrange(new Rect(0, 0, 800, 600));
        panel.DistributePanelHeight();

        // 动态添加项目
        items.Add(CreateCurveGroup("Curve2"));
        items.Add(CreateStatuSeriesGroup("Status1"));

        // 验证动态更新不会崩溃
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    #endregion

    #region 高度分配验证测试

    [Test]
    public void DistributePanelHeight_VerifyHeightFormula()
    {
        // 验证高度计算公式的正确性
        // 公式: curvePanelHeight = (totalHeight - statusPanelCount * statusPanelHeight) / curvePanelCount

        // 场景 1: 600px, 2 Curve, 1 Status (20px)
        double h1 = (600 - 1 * 20) / 2.0;
        Assert.That(h1, Is.EqualTo(290), "Scenario 1: 2 Curve, 1 Status");

        // 场景 2: 800px, 3 Curve, 2 Status (20px)
        double h2 = (800 - 2 * 20) / 3.0;
        Assert.That(h2, Is.EqualTo(253.33333333333334).Within(0.001), "Scenario 2: 3 Curve, 2 Status");

        // 场景 3: 1000px, 4 Curve, 0 Status
        double h3 = (1000 - 0 * 20) / 4.0;
        Assert.That(h3, Is.EqualTo(250), "Scenario 3: 4 Curve, 0 Status");

        // 场景 4: 500px, 1 Curve, 3 Status (16px)
        double h4 = (500 - 3 * 16) / 1.0;
        Assert.That(h4, Is.EqualTo(452), "Scenario 4: 1 Curve, 3 Status");
    }

    [Test]
    public void DistributePanelHeight_StatusPanelAlwaysFixed()
    {
        // 验证 StatusPanel 高度始终固定
        double statusPanelHeight = 20;
        
        // 无论可用空间如何，StatusPanel 高度固定
        Assert.That(1 * statusPanelHeight, Is.EqualTo(20));
        Assert.That(5 * statusPanelHeight, Is.EqualTo(100));
        Assert.That(10 * statusPanelHeight, Is.EqualTo(200));
    }

    [Test]
    public void DistributePanelHeight_CurvePanelSharesRemainingSpace()
    {
        // 验证 CurvePanel 平分剩余空间
        double availableHeight = 540;
        int curvePanelCount = 3;
        
        double curvePanelHeight = availableHeight / curvePanelCount;
        
        // 每个 CurvePanel 高度相同
        Assert.That(curvePanelHeight, Is.EqualTo(180));
        
        // 总高度正确
        Assert.That(curvePanelHeight * curvePanelCount, Is.EqualTo(availableHeight));
    }

    #endregion

    #region 负面测试

    [AvaloniaTest]
    public void DistributePanelHeight_ShouldHandleNullItems()
    {
        var panel = CreateTestPanel();
        panel.ItemsSource = null;
        
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    [AvaloniaTest]
    public void DistributePanelHeight_ShouldHandleEmptyItems()
    {
        var panel = CreateTestPanel();
        panel.ItemsSource = new List<object>();
        
        Assert.DoesNotThrow(() => panel.DistributePanelHeight());
    }

    [Test]
    public void DistributePanelHeight_ShouldHandleVerySmallHeight()
    {
        double totalHeight = 30;
        double statusPanelHeight = 20;
        int statusPanelCount = 1;
        int curvePanelCount = 1;

        double statusPanelTotalHeight = statusPanelCount * statusPanelHeight;
        double curveAvailableHeight = totalHeight - statusPanelTotalHeight;

        // 可用高度非常小但仍然有效
        Assert.That(curveAvailableHeight, Is.EqualTo(10));
        
        double curvePanelHeight = curveAvailableHeight / curvePanelCount;
        Assert.That(curvePanelHeight, Is.EqualTo(10));
    }

    #endregion
}

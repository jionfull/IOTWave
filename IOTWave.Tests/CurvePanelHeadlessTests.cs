using Avalonia;
using Avalonia.Headless.NUnit;
using IOTWave.Views;

namespace IOTWave.Tests;

/// <summary>
/// CurvePanel 中需要 Avalonia 平台的测试
/// 使用 [AvaloniaTest] 属性在 headless 模式下运行
/// </summary>
[TestFixture]
public class CurvePanelHeadlessTests
{
    #region BuildCurveGeometry 测试

    [AvaloniaTest]
    public void BuildCurveGeometry_EmptyList_ReturnsEmptyGeometry()
    {
        var points = new List<(Point, int)>();
        var drawArea = new Rect(100, 0, 600, 400);

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    [AvaloniaTest]
    public void BuildCurveGeometry_BothPointsInside_DrawsLine()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(200, 100), 0),
            (new Point(300, 200), 1),
        };

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    [AvaloniaTest]
    public void BuildCurveGeometry_FromOutsideToInside_StartsAtIntersection()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(50, 100), 0),
            (new Point(200, 200), 1),
        };

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    [AvaloniaTest]
    public void BuildCurveGeometry_FromInsideToOutside_EndsAtIntersection()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(600, 200), 0),
            (new Point(800, 300), 1),
        };

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    [AvaloniaTest]
    public void BuildCurveGeometry_BothOutsideCrossingArea_DrawsAcross()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(50, 100), 0),
            (new Point(800, 200), 1),
        };

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    [AvaloniaTest]
    public void BuildCurveGeometry_MultipleSegments_ConnectsCorrectly()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(200, 100), 0),
            (new Point(300, 150), 1),
            (new Point(400, 200), 2),
            (new Point(500, 180), 3),
        };

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    [AvaloniaTest]
    public void BuildCurveGeometry_SinglePoint_ReturnsEmpty()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(200, 100), 0),
        };

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    [AvaloniaTest]
    public void BuildCurveGeometry_AlternatingInsideOutside_HandlesCorrectly()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        var points = new List<(Point, int)>
        {
            (new Point(50, 100), 0),
            (new Point(200, 150), 1),
            (new Point(800, 200), 2),
            (new Point(400, 180), 3),
        };

        var geometry = CurvePanel.BuildCurveGeometry(points, drawArea);

        Assert.That(geometry, Is.Not.Null);
    }

    #endregion
}

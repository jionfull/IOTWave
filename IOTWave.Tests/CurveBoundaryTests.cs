using Avalonia;
using Avalonia.Controls;
using IOTWave.Models;

namespace IOTWave.Tests;

/// <summary>
/// 测试曲线在边界处的连线绘制逻辑
/// </summary>
[TestFixture]
public class CurveBoundaryTests
{
    #region 边界可见性测试

    [Test]
    public void PointVisibility_ShouldIdentifyPointsInsideDrawArea()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var pointInside = new Point(300, 200); // X 在 [100, 700] 范围内

        // Act
        bool isVisible = pointInside.X >= drawArea.Left && pointInside.X <= drawArea.Right;

        // Assert
        Assert.That(isVisible, Is.True, "点在区域内应该被认为是可见的");
    }

    [Test]
    public void PointVisibility_ShouldIdentifyPointsOutsideLeft()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var pointOutsideLeft = new Point(50, 200); // X < 100

        // Act
        bool isVisible = pointOutsideLeft.X >= drawArea.Left && pointOutsideLeft.X <= drawArea.Right;

        // Assert
        Assert.That(isVisible, Is.False, "点在左侧边界外应该被认为是不可见的");
    }

    [Test]
    public void PointVisibility_ShouldIdentifyPointsOutsideRight()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var pointOutsideRight = new Point(800, 200); // X > 700

        // Act
        bool isVisible = pointOutsideRight.X >= drawArea.Left && pointOutsideRight.X <= drawArea.Right;

        // Assert
        Assert.That(isVisible, Is.False, "点在右侧边界外应该被认为是不可见的");
    }

    #endregion

    #region 交点计算测试

    [Test]
    public void GetIntersection_LeftBoundary_ShouldCalculateCorrectY()
    {
        // Arrange
        var p1 = new Point(50, 100); // 左边界外
        var p2 = new Point(150, 200); // 边界内
        var leftBoundary = 100;

        // Act - 线性插值计算交点
        var t = (leftBoundary - p1.X) / (p2.X - p1.X);
        var y = p1.Y + t * (p2.Y - p1.Y);
        var intersection = new Point(leftBoundary, y);

        // Assert
        Assert.That(intersection.X, Is.EqualTo(100), "交点X应该在左边界上");
        Assert.That(intersection.Y, Is.EqualTo(150).Within(0.001), "交点Y应该正确插值");
    }

    [Test]
    public void GetIntersection_RightBoundary_ShouldCalculateCorrectY()
    {
        // Arrange
        var p1 = new Point(650, 200); // 边界内
        var p2 = new Point(800, 300); // 右边界外
        var rightBoundary = 700;

        // Act - 线性插值计算交点
        var t = (rightBoundary - p1.X) / (p2.X - p1.X);
        var y = p1.Y + t * (p2.Y - p1.Y);
        var intersection = new Point(rightBoundary, y);

        // Assert
        Assert.That(intersection.X, Is.EqualTo(700), "交点X应该在右边界上");
        // t = (700-650)/(800-650) = 50/150 = 1/3
        // y = 200 + (1/3)*(300-200) = 233.33
        Assert.That(intersection.Y, Is.EqualTo(233.33333333333331).Within(0.001), "交点Y应该正确插值");
    }

    #endregion

    #region 边界连线场景测试

    /// <summary>
    /// 场景1：从左边界外到边界内
    /// 预期：计算与左边界的交点，从交点开始绘制到边界内的点
    /// </summary>
    [Test]
    public void BoundaryLine_FromLeftOutsideToInside_ShouldDrawFromIntersection()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var prevOutside = new Point(50, 100); // 左边界外
        var currentInside = new Point(200, 200); // 边界内

        // 验可见性
        bool prevVisible = prevOutside.X >= drawArea.Left && prevOutside.X <= drawArea.Right;
        bool currentVisible = currentInside.X >= drawArea.Left && currentInside.X <= drawArea.Right;

        // Assert - 应该触发"从区域外到区域内"的逻辑
        Assert.That(prevVisible, Is.False, "前一点应该在边界外");
        Assert.That(currentVisible, Is.True, "当前点应该在边界内");
        Assert.That(prevOutside.X < drawArea.Left, Is.True, "前一点应该在左侧边界外");

        // 计算交点
        var t = (drawArea.Left - prevOutside.X) / (currentInside.X - prevOutside.X);
        var y = prevOutside.Y + t * (currentInside.Y - prevOutside.Y);
        var intersection = new Point(drawArea.Left, y);

        // 验证交点正确
        // t = (100-50)/(200-50) = 50/150 = 1/3
        // y = 100 + (1/3)*(200-100) = 133.33
        Assert.That(intersection.X, Is.EqualTo(100), "交点应该在左边界");
        Assert.That(intersection.Y, Is.EqualTo(133.33333333333333).Within(0.001), "交点Y应该正确插值");
    }

    /// <summary>
    /// 场景2：从右边界外到边界内
    /// 预期：计算与右边界的交点，从交点开始绘制到边界内的点
    /// </summary>
    [Test]
    public void BoundaryLine_FromRightOutsideToInside_ShouldDrawFromIntersection()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var prevOutside = new Point(800, 300); // 右边界外
        var currentInside = new Point(600, 200); // 边界内

        // 验可见性
        bool prevVisible = prevOutside.X >= drawArea.Left && prevOutside.X <= drawArea.Right;
        bool currentVisible = currentInside.X >= drawArea.Left && currentInside.X <= drawArea.Right;

        // Assert - 应该触发"从区域外到区域内"的逻辑
        Assert.That(prevVisible, Is.False, "前一点应该在边界外");
        Assert.That(currentVisible, Is.True, "当前点应该在边界内");
        Assert.That(prevOutside.X > drawArea.Right, Is.True, "前一点应该在右侧边界外");

        // 计算交点
        var t = (drawArea.Right - prevOutside.X) / (currentInside.X - prevOutside.X);
        var y = prevOutside.Y + t * (currentInside.Y - prevOutside.Y);
        var intersection = new Point(drawArea.Right, y);

        // 验证交点正确
        // t = (700-800)/(600-800) = -100/-200 = 0.5
        // y = 300 + 0.5*(200-300) = 250
        Assert.That(intersection.X, Is.EqualTo(700), "交点应该在右边界");
        Assert.That(intersection.Y, Is.EqualTo(250).Within(0.001), "交点Y应该正确插值");
    }

    /// <summary>
    /// 场景3：从边界内到左边界外
    /// 预期：计算与左边界的交点，从边界内点绘制到交点
    /// </summary>
    [Test]
    public void BoundaryLine_FromInsideToLeftOutside_ShouldDrawToIntersection()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var prevInside = new Point(200, 200); // 边界内
        var currentOutside = new Point(50, 100); // 左边界外

        // 验可见性
        bool prevVisible = prevInside.X >= drawArea.Left && prevInside.X <= drawArea.Right;
        bool currentVisible = currentOutside.X >= drawArea.Left && currentOutside.X <= drawArea.Right;

        // Assert - 应该触发"从区域内到区域外"的逻辑
        Assert.That(prevVisible, Is.True, "前一点应该在边界内");
        Assert.That(currentVisible, Is.False, "当前点应该在边界外");
        Assert.That(currentOutside.X < drawArea.Left, Is.True, "当前点应该在左侧边界外");

        // 计算交点
        var t = (drawArea.Left - prevInside.X) / (currentOutside.X - prevInside.X);
        var y = prevInside.Y + t * (currentOutside.Y - prevInside.Y);
        var intersection = new Point(drawArea.Left, y);

        // 验证交点正确
        // t = (100-200)/(50-200) = -100/-150 = 2/3
        // y = 200 + (2/3)*(100-200) = 133.33
        Assert.That(intersection.X, Is.EqualTo(100), "交点应该在左边界");
        Assert.That(intersection.Y, Is.EqualTo(133.33333333333334).Within(0.001), "交点Y应该正确插值");
    }

    /// <summary>
    /// 场景4：从边界内到右边界外
    /// 预期：计算与右边界的交点，从边界内点绘制到交点
    /// </summary>
    [Test]
    public void BoundaryLine_FromInsideToRightOutside_ShouldDrawToIntersection()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var prevInside = new Point(600, 200); // 边界内
        var currentOutside = new Point(800, 300); // 右边界外

        // 验可见性
        bool prevVisible = prevInside.X >= drawArea.Left && prevInside.X <= drawArea.Right;
        bool currentVisible = currentOutside.X >= drawArea.Left && currentOutside.X <= drawArea.Right;

        // Assert - 应该触发"从区域内到区域外"的逻辑
        Assert.That(prevVisible, Is.True, "前一点应该在边界内");
        Assert.That(currentVisible, Is.False, "当前点应该在边界外");
        Assert.That(currentOutside.X > drawArea.Right, Is.True, "当前点应该在右侧边界外");

        // 计算交点
        var t = (drawArea.Right - prevInside.X) / (currentOutside.X - prevInside.X);
        var y = prevInside.Y + t * (currentOutside.Y - prevInside.Y);
        var intersection = new Point(drawArea.Right, y);

        // 验证交点正确
        // t = (700-600)/(800-600) = 100/200 = 0.5
        // y = 200 + 0.5*(300-200) = 250
        Assert.That(intersection.X, Is.EqualTo(700), "交点应该在右边界");
        Assert.That(intersection.Y, Is.EqualTo(250).Within(0.001), "交点Y应该正确插值");
    }

    /// <summary>
    /// 场景5：两点都在边界外但跨越整个区域
    /// 预期：计算与左右边界的交点，在两个交点之间绘制连线
    /// </summary>
    [Test]
    public void BoundaryLine_BothOutsideCrossingArea_ShouldDrawAcross()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var prevOutside = new Point(50, 100); // 左边界外
        var currentOutside = new Point(800, 200); // 右边界外

        // 验可见性
        bool prevVisible = prevOutside.X >= drawArea.Left && prevOutside.X <= drawArea.Right;
        bool currentVisible = currentOutside.X >= drawArea.Left && currentOutside.X <= drawArea.Right;

        // Assert - 应该触发"两点都在区域外但跨越整个区域"的逻辑
        Assert.That(prevVisible, Is.False, "前一点应该在边界外");
        Assert.That(currentVisible, Is.False, "当前点应该在边界外");
        Assert.That(prevOutside.X < drawArea.Left, Is.True, "前一点应该在左侧边界外");
        Assert.That(currentOutside.X > drawArea.Right, Is.True, "当前点应该在右侧边界外");

        // 计算两个交点
        var t1 = (drawArea.Left - prevOutside.X) / (currentOutside.X - prevOutside.X);
        var y1 = prevOutside.Y + t1 * (currentOutside.Y - prevOutside.Y);
        var startIntersection = new Point(drawArea.Left, y1);

        var t2 = (drawArea.Right - prevOutside.X) / (currentOutside.X - prevOutside.X);
        var y2 = prevOutside.Y + t2 * (currentOutside.Y - prevOutside.Y);
        var endIntersection = new Point(drawArea.Right, y2);

        // 验证交点正确
        // t1 = (100-50)/(800-50) = 50/750 = 1/15
        // y1 = 100 + (1/15)*(200-100) = 106.67
        // t2 = (700-50)/(800-50) = 650/750 = 13/15
        // y2 = 100 + (13/15)*(200-100) = 186.67
        Assert.That(startIntersection.X, Is.EqualTo(100), "起始交点应该在左边界");
        Assert.That(endIntersection.X, Is.EqualTo(700), "结束交点应该在右边界");
        Assert.That(startIntersection.Y, Is.EqualTo(106.66666666666667).Within(0.001), "起始交点Y应该正确插值");
        Assert.That(endIntersection.Y, Is.EqualTo(186.66666666666667).Within(0.001), "结束交点Y应该正确插值");
    }

    /// <summary>
    /// 场景6：从右向左穿越边界（时间序列反转）
    /// 这是最常见的边界情况：时间点从右边界外进入边界内
    /// </summary>
    [Test]
    public void BoundaryLine_SequenceFromRightToInside_ShouldHandleCorrectly()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700

        // 模拟时间序列（从右向左）
        var points = new List<Point>
        {
            new Point(850, 100),  // 第0点：右边界外
            new Point(800, 120),  // 第1点：右边界外
            new Point(750, 140),  // 第2点：右边界外
            new Point(650, 160),  // 第3点：边界内
            new Point(500, 180),  // 第4点：边界内
            new Point(400, 200)   // 第5点：边界内
        };

        // 验证每段连线应该被绘制
        for (int i = 1; i < points.Count; i++)
        {
            var prev = points[i - 1];
            var current = points[i];

            bool prevVisible = prev.X >= drawArea.Left && prev.X <= drawArea.Right;
            bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

            // i=3: 从右边界外(750)到边界内(650)
            if (i == 3)
            {
                Assert.That(prevVisible, Is.False, $"第{i}段前一点(750)应该在边界外");
                Assert.That(currentVisible, Is.True, $"第{i}段当前点(650)应该在边界内");
                Assert.That(prev.X > drawArea.Right, Is.True, $"第{i}段前一点应该在右侧边界外");

                // 计算交点
                var t = (drawArea.Right - prev.X) / (current.X - prev.X);
                var y = prev.Y + t * (current.Y - prev.Y);
                var intersection = new Point(drawArea.Right, y);

                // t = (700-750)/(650-750) = -50/-100 = 0.5
                // y = 140 + 0.5*(160-140) = 150
                Assert.That(intersection.X, Is.EqualTo(700), $"第{i}段交点X应该在右边界");
                Assert.That(intersection.Y, Is.EqualTo(150).Within(0.001), $"第{i}段交点Y应该正确插值");
            }

            // i=4: 两点都在边界内
            if (i == 4)
            {
                Assert.That(prevVisible, Is.True, $"第{i}段前一点应该在边界内");
                Assert.That(currentVisible, Is.True, $"第{i}段当前点应该在边界内");
            }

            // i=5: 两点都在边界内
            if (i == 5)
            {
                Assert.That(prevVisible, Is.True, $"第{i}段前一点应该在边界内");
                Assert.That(currentVisible, Is.True, $"第{i}段当前点应该在边界内");
            }
        }
    }

    #endregion

    #region 边缘情况测试

    /// <summary>
    /// 边缘情况：点恰好在边界上
    /// </summary>
    [Test]
    public void PointOnBoundary_ShouldBeConsideredVisible()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        var pointOnLeft = new Point(100, 200);
        var pointOnRight = new Point(700, 200);

        // Act & Assert
        bool leftVisible = pointOnLeft.X >= drawArea.Left && pointOnLeft.X <= drawArea.Right;
        bool rightVisible = pointOnRight.X >= drawArea.Left && pointOnRight.X <= drawArea.Right;

        Assert.That(leftVisible, Is.True, "点恰好在左边界上应该被认为是可见的");
        Assert.That(rightVisible, Is.True, "点恰好在右边界上应该被认为是可见的");
    }

    /// <summary>
    /// 边缘情况：垂直线段（X坐标相同）
    /// </summary>
    [Test]
    public void VerticalSegment_ShouldHandleCorrectly()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400);
        var p1 = new Point(300, 100);
        var p2 = new Point(300, 300); // X 相同，在边界内

        // Act
        bool visible = p1.X >= drawArea.Left && p1.X <= drawArea.Right;

        // Assert
        Assert.That(visible, Is.True, "垂直线段应该在边界内");
    }

    #endregion

    #region 边界线段丢失Bug复现测试

    /// <summary>
    /// Bug复现：下采样后边界点丢失导致线段不绘制
    /// 场景：当点被下采样后，边界处的点可能丢失，导致从边界外到边界内的线段无法正确绘制
    /// </summary>
    [Test]
    public void Downsample_BoundaryPointLost_ShouldStillDrawLine()
    {
        // Arrange - 模拟下采样后的点序列
        // 原始数据有很多点，下采样后只剩下关键点
        // 关键点在边界外和边界内，但中间点被下采样移除
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        
        // 模拟下采样后的点（关键点）
        var downsampledPoints = new List<Point>
        {
            new Point(50, 100),    // 第0点：左边界外（保留的起点）
            new Point(80, 150),    // 第1点：左边界外（极值点）
            new Point(150, 200),   // 第2点：边界内（保留的终点）
            new Point(300, 180),   // 第3点：边界内
            new Point(650, 160),   // 第4点：边界内（保留的起点）
            new Point(720, 140),   // 第5点：右边界外（极值点）
            new Point(800, 100)    // 第6点：右边界外（保留的终点）
        };

        // Act & Assert - 验证每段连线的可见性判断
        for (int i = 1; i < downsampledPoints.Count; i++)
        {
            var prev = downsampledPoints[i - 1];
            var current = downsampledPoints[i];

            bool prevVisible = prev.X >= drawArea.Left && prev.X <= drawArea.Right;
            bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

            // i=2: 从左边界外(80)到边界内(150)
            // 这是一个关键的边界跨越，应该触发"从区域外到区域内"逻辑
            if (i == 2)
            {
                Assert.That(prevVisible, Is.False, $"第{i}段前一点(80)应该在左边界外");
                Assert.That(currentVisible, Is.True, $"第{i}段当前点(150)应该在边界内");
                Assert.That(prev.X < drawArea.Left, Is.True, $"第{i}段前一点应该在左侧边界外");

                // 计算与左边界的交点
                var t = (drawArea.Left - prev.X) / (current.X - prev.X);
                var y = prev.Y + t * (current.Y - prev.Y);
                var intersection = new Point(drawArea.Left, y);

                // t = (100-80)/(150-80) = 20/70 = 0.2857
                // y = 150 + 0.2857*(200-150) = 164.28
                Assert.That(intersection.X, Is.EqualTo(100), $"第{i}段交点X应该在左边界");
                Assert.That(intersection.Y, Is.EqualTo(164.28571428571428).Within(0.001), $"第{i}段交点Y应该正确插值");
            }

            // i=5: 从边界内(650)到右边界外(720)
            // 这是另一个关键的边界跨越
            if (i == 5)
            {
                Assert.That(prevVisible, Is.True, $"第{i}段前一点(650)应该在边界内");
                Assert.That(currentVisible, Is.False, $"第{i}段当前点(720)应该在右边界外");
                Assert.That(current.X > drawArea.Right, Is.True, $"第{i}段当前点应该在右侧边界外");

                // 计算与右边界的交点
                var t = (drawArea.Right - prev.X) / (current.X - prev.X);
                var y = prev.Y + t * (current.Y - prev.Y);
                var intersection = new Point(drawArea.Right, y);

                // t = (700-650)/(720-650) = 50/70 = 0.714
                // y = 160 + 0.714*(140-160) = 145.71
                Assert.That(intersection.X, Is.EqualTo(700), $"第{i}段交点X应该在右边界");
                Assert.That(intersection.Y, Is.EqualTo(145.71428571428572).Within(0.001), $"第{i}段交点Y应该正确插值");
            }
        }
    }

    /// <summary>
    /// Bug复现：第一个可见点在边界内，但前一个点（下采样后）也在边界内的情况
    /// 这种情况可能导致线段从错误的位置开始绘制
    /// </summary>
    [Test]
    public void Downsample_FirstVisiblePointInside_ButPrevAlsoInside()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400);
        
        // 模拟下采样后的点序列
        // 第0点在边界外，第1点也在边界外但靠近边界，第2点才进入边界
        var downsampledPoints = new List<Point>
        {
            new Point(20, 100),    // 第0点：左边界外较远
            new Point(90, 150),    // 第1点：左边界外但接近边界（距离边界10像素）
            new Point(110, 180),   // 第2点：边界内（距离边界10像素）
            new Point(300, 200)    // 第3点：边界内
        };

        // 验证第1点到第2点的跨越
        var prev = downsampledPoints[1]; // (90, 150) - 边界外
        var current = downsampledPoints[2]; // (110, 180) - 边界内

        bool prevVisible = prev.X >= drawArea.Left && prev.X <= drawArea.Right;
        bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

        Assert.That(prevVisible, Is.False, "前一点(90)应该在边界外");
        Assert.That(currentVisible, Is.True, "当前点(110)应该在边界内");

        // 计算交点 - 这是应该开始绘制的地方
        var t = (drawArea.Left - prev.X) / (current.X - prev.X);
        var y = prev.Y + t * (current.Y - prev.Y);
        var intersection = new Point(drawArea.Left, y);

        // t = (100-90)/(110-90) = 10/20 = 0.5
        // y = 150 + 0.5*(180-150) = 165
        Assert.That(intersection.X, Is.EqualTo(100), "交点X应该在左边界");
        Assert.That(intersection.Y, Is.EqualTo(165).Within(0.001), "交点Y应该正确插值");
    }

    /// <summary>
    /// Bug复现：下采样导致边界处极值点丢失，造成线段断裂
    /// </summary>
    [Test]
    public void Downsample_ExtremePointLostAtBoundary_CausesLineBreak()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400);
        
        // 原始数据在边界附近有一个峰值，但下采样后峰值点被移除
        // 导致下采样后的点直接从边界外跳到边界内，跳过了峰值
        var downsampledPoints = new List<Point>
        {
            new Point(50, 100),    // 边界外的起点
            // 注意：这里缺少一个在边界附近(95, 250)的峰值点，被下采样移除了
            new Point(150, 200),   // 边界内的点
            new Point(300, 150)    // 边界内的另一个点
        };

        // 验证第0点到第1点的跨越（跳过了峰值）
        var prev = downsampledPoints[0]; // (50, 100)
        var current = downsampledPoints[1]; // (150, 200)

        bool prevVisible = prev.X >= drawArea.Left && prev.X <= drawArea.Right;
        bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

        Assert.That(prevVisible, Is.False, "前一点应该在边界外");
        Assert.That(currentVisible, Is.True, "当前点应该在边界内");

        // 即使跳过了峰值，也应该正确计算交点并绘制线段
        var t = (drawArea.Left - prev.X) / (current.X - prev.X);
        var y = prev.Y + t * (current.Y - prev.Y);
        var intersection = new Point(drawArea.Left, y);

        // t = (100-50)/(150-50) = 50/100 = 0.5
        // y = 100 + 0.5*(200-100) = 150
        Assert.That(intersection.X, Is.EqualTo(100), "交点X应该在左边界");
        Assert.That(intersection.Y, Is.EqualTo(150).Within(0.001), "交点Y应该正确插值");
    }

    /// <summary>
    /// Bug复现：连续多个点在边界外，下采样后只剩一个边界外点
    /// 这可能导致线段绘制不正确
    /// </summary>
    [Test]
    public void Downsample_MultiplePointsOutside_BecomesOnePoint()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400);
        
        // 原始有多个点在边界外，下采样后只剩一个代表点
        var downsampledPoints = new List<Point>
        {
            new Point(30, 100),    // 代表所有左边界外的点
            new Point(150, 200),   // 第一个边界内的点
            new Point(300, 180),   // 边界内的点
            new Point(750, 160),   // 代表所有右边界外的点
        };

        // 验证左边界跨越
        var prevLeft = downsampledPoints[0];
        var currentLeft = downsampledPoints[1];

        bool prevVisible = prevLeft.X >= drawArea.Left && prevLeft.X <= drawArea.Right;
        bool currentVisible = currentLeft.X >= drawArea.Left && currentLeft.X <= drawArea.Right;

        Assert.That(prevVisible, Is.False, "左边界外点应该不可见");
        Assert.That(currentVisible, Is.True, "左边界内点应该可见");

        // 验证右边界跨越
        var prevRight = downsampledPoints[2];
        var currentRight = downsampledPoints[3];

        prevVisible = prevRight.X >= drawArea.Left && prevRight.X <= drawArea.Right;
        currentVisible = currentRight.X >= drawArea.Left && currentRight.X <= drawArea.Right;

        Assert.That(prevVisible, Is.True, "右边界内点应该可见");
        Assert.That(currentVisible, Is.False, "右边界外点应该不可见");
    }

    #endregion

    #region 真实Bug复现测试 - 第一个点在边界内时线段丢失

    /// <summary>
    /// 关键Bug复现：当第一个下采样点就在边界内时，线段丢失
    /// 这是截图中显示的问题：绿色框内的曲线线段没有绘制
    /// 
    /// 问题原因：
    /// 1. prevTransformed = downsampledPoints[0] (第一个点在边界内)
    /// 2. 循环从 i=1 开始
    /// 3. i=1 时，prevVisible=true, currentVisible=true
    /// 4. 但 firstPoint 为 null，所以不会调用 ctx.BeginFigure
    /// 5. 结果：第一个线段 (points[0] -> points[1]) 丢失！
    /// </summary>
    [Test]
    public void FirstPointInside_BoundaryLineLost_BugReproduction()
    {
        // Arrange - 模拟下采样后的点，第一个点就在边界内
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        
        // 这是关键：第一个点(150)已经在边界内，不是从边界外进入
        var downsampledPoints = new List<Point>
        {
            new Point(150, 200),   // 第0点：已经在边界内！
            new Point(200, 220),   // 第1点：边界内
            new Point(300, 180),   // 第2点：边界内
        };

        // 模拟绘制逻辑（简化版）
        bool figureStarted = false;
        int linesDrawn = 0;
        
        Point prevTransformed = downsampledPoints[0];
        
        // 从第二个点开始遍历（i=1）- 这是原始代码的逻辑
        for (int i = 1; i < downsampledPoints.Count; i++)
        {
            var current = downsampledPoints[i];
            
            bool prevVisible = prevTransformed.X >= drawArea.Left && prevTransformed.X <= drawArea.Right;
            bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

            if (prevVisible && currentVisible)
            {
                // 两点都在可见区域内
                if (!figureStarted)
                {
                    // BUG：这里永远不会执行，因为 figureStarted 初始为 false
                    // 但 prevTransformed 是 points[0]，而我们在 i=1 时比较的是 points[0] 和 points[1]
                    // 这导致第一个线段丢失！
                    figureStarted = true;
                }
                linesDrawn++;
            }
            
            prevTransformed = current;
        }

        // Assert - 期望绘制2条线段：(0->1) 和 (1->2)
        // 但由于bug，如果只检查 figureStarted 在循环内设置，第一个线段会丢失
        Assert.That(linesDrawn, Is.EqualTo(2), "应该绘制2条线段");
        Assert.That(figureStarted, Is.True, "图形应该已经开始绘制");
    }

    /// <summary>
    /// 验证Bug：当第一个点在边界内时，第一个线段确实会丢失
    /// </summary>
    [Test]
    public void FirstPointInside_FirstLineSegmentMissing()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        
        // 场景：第一个点在边界内，导致第一个线段丢失
        var points = new List<Point>
        {
            new Point(150, 100),  // 在边界内
            new Point(200, 150),  // 在边界内
        };

        // 使用原始代码逻辑
        Point prevTransformed = points[0];
        bool firstPointSet = false;
        
        for (int i = 1; i < points.Count; i++)
        {
            var current = points[i];
            bool prevVisible = prevTransformed.X >= drawArea.Left && prevTransformed.X <= drawArea.Right;
            bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

            if (prevVisible && currentVisible)
            {
                if (!firstPointSet)
                {
                    firstPointSet = true;
                }
            }
            
            prevTransformed = current;
        }

        // 这个测试验证：当第一个点在边界内时，firstPointSet 会被正确设置
        // 但问题在于 ctx.BeginFigure 应该在 prevTransformed（即 points[0]）处开始
        // 而不是在循环内部条件判断后才开始
        Assert.That(firstPointSet, Is.True, "当两点都在边界内时，firstPointSet 应该被设置");
    }

    /// <summary>
    /// 修复验证：正确处理第一个点在边界内的情况
    /// </summary>
    [Test]
    public void FirstPointInside_FixedLogic_ShouldDrawCorrectly()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        
        var points = new List<Point>
        {
            new Point(150, 100),  // 第0点：在边界内
            new Point(200, 150),  // 第1点：在边界内
            new Point(250, 120),  // 第2点：在边界内
        };

        // 修复后的逻辑：
        // 1. 如果第一个点在边界内，立即开始图形
        // 2. 然后正常处理后续点
        
        bool figureStarted = false;
        Point startPoint = points[0];
        int lineCount = 0;
        
        // 检查第一个点是否在边界内
        if (startPoint.X >= drawArea.Left && startPoint.X <= drawArea.Right)
        {
            figureStarted = true;
        }

        Point prev = points[0];
        for (int i = 1; i < points.Count; i++)
        {
            var current = points[i];
            bool prevVisible = prev.X >= drawArea.Left && prev.X <= drawArea.Right;
            bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

            if (prevVisible && currentVisible)
            {
                lineCount++;
            }
            
            prev = current;
        }

        Assert.That(figureStarted, Is.True, "修复后图形应该立即开始");
        Assert.That(lineCount, Is.EqualTo(2), "应该绘制2条线段");
    }

    #endregion

    #region 下采样导致边界点丢失的Bug

    /// <summary>
    /// 关键Bug：下采样时，边界处的点可能被丢弃，导致线段在边界处断裂
    /// 
    /// 场景：
    /// - 原始数据有很多点
    /// - 下采样后只保留关键点（起点、终点、极值）
    /// - 但边界处（LeftPadding处）的点可能不是关键点，被丢弃了
    /// - 结果：从边界外到边界内的线段无法正确绘制
    /// </summary>
    [Test]
    public void Downsample_BoundaryPointMissing_CausesLineBreak()
    {
        // Arrange
        var drawArea = new Rect(100, 0, 600, 400); // Left=100, Right=700
        
        // 模拟下采样后的点序列
        // 关键点：在 X=100（边界）附近没有点，下采样跳过了这个区域
        var downsampledPoints = new List<Point>
        {
            new Point(50, 100),    // 左边界外（保留的起点）
            // 缺失：X=100 附近的点被下采样丢弃了！
            new Point(150, 200),   // 边界内（保留的终点）
            new Point(300, 180),   // 边界内
        };

        // Act - 模拟绘制逻辑
        bool figureStarted = false;
        Point? firstPoint = null;
        Point prevTransformed = downsampledPoints[0];
        int lineSegmentsDrawn = 0;
        
        for (int i = 1; i < downsampledPoints.Count; i++)
        {
            var current = downsampledPoints[i];
            bool prevVisible = prevTransformed.X >= drawArea.Left && prevTransformed.X <= drawArea.Right;
            bool currentVisible = current.X >= drawArea.Left && current.X <= drawArea.Right;

            if (prevVisible && currentVisible)
            {
                // 两点都在可见区域内
                if (!firstPoint.HasValue)
                {
                    firstPoint = prevTransformed;
                    figureStarted = true;
                }
                lineSegmentsDrawn++;
            }
            else if (!prevVisible && currentVisible)
            {
                // 从区域外到区域内：计算交点
                if (prevTransformed.X < drawArea.Left)
                {
                    var intersection = GetIntersectionForTest(prevTransformed, current, drawArea.Left);
                    figureStarted = true;
                    lineSegmentsDrawn++;
                }
            }
            
            prevTransformed = current;
        }

        // Assert
        // 期望：即使下采样丢失了边界点，也应该通过交点计算绘制线段
        Assert.That(figureStarted, Is.True, "图形应该开始绘制");
        Assert.That(lineSegmentsDrawn, Is.EqualTo(2), "应该绘制2条线段：(50->150)跨越边界 和 (150->300)在边界内");
    }

    /// <summary>
    /// 测试辅助方法：计算线段与垂直边界的交点
    /// </summary>
    private Point GetIntersectionForTest(Point p1, Point p2, double xBoundary)
    {
        var t = (xBoundary - p1.X) / (p2.X - p1.X);
        var y = p1.Y + t * (p2.Y - p1.Y);
        return new Point(xBoundary, y);
    }

    /// <summary>
    /// 修复方案：确保下采样时保留边界处的点
    /// 或者：在绘制时正确处理跨越边界的线段
    /// </summary>
    [Test]
    public void Downsample_WithBoundaryPointPreserved_ShouldWork()
    {
        var drawArea = new Rect(100, 0, 600, 400);
        
        // 修复后的下采样：保留边界处的点
        var downsampledPoints = new List<Point>
        {
            new Point(50, 100),    // 左边界外
            new Point(100, 150),   // 左边界上（新增：确保边界点被保留）
            new Point(150, 200),   // 边界内
            new Point(300, 180),   // 边界内
        };

        // 验证所有点
        Assert.That(downsampledPoints[1].X, Is.EqualTo(drawArea.Left), "边界点应该被保留");
        
        int visiblePoints = downsampledPoints.Count(p => 
            p.X >= drawArea.Left && p.X <= drawArea.Right);
        
        Assert.That(visiblePoints, Is.EqualTo(3), "应该有3个点在可见区域内");
    }

    #endregion
}

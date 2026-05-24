using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphBuilder.Constants;
using GraphBuilder.Models;

namespace GraphBuilder.Rendering;

/// <summary>
/// Статический класс для создания визуальных элементов дуги:
/// основная линия, невидимая линия большей толщины,
/// текст предиката и стрелка-наконечник.
/// </summary>
public static class EdgeRenderer
{
    // Создаёт все визуальные элементы дуги и возвращает контейнер для них.
    public static EdgeVisuals Create(GraphEdge edge)
    {
        var mainLine = new Line
        {
            X1 = edge.X1, Y1 = edge.Y1,
            X2 = edge.X2, Y2 = edge.Y2,
            Stroke = AppConstants.s_DefaultEdgeBrush,
            StrokeThickness = 2,
            Tag = edge
        };
        Panel.SetZIndex(mainLine, 1);

        var hitTestLine = new Line
        {
            X1 = edge.X1, Y1 = edge.Y1,
            X2 = edge.X2, Y2 = edge.Y2,
            Stroke = Brushes.Transparent,
            StrokeThickness = AppConstants.HitTestTolerance * 2,
            IsHitTestVisible = true,
            Tag = edge
        };
        Panel.SetZIndex(hitTestLine, 2);

        var predicateText = new TextBlock
        {
            Text = edge.Predicate.ToString(),
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Background = Brushes.White,
            Padding = new Thickness(2)
        };
        Panel.SetZIndex(predicateText, 3);
        UpdatePredicatePosition(predicateText, edge);
        
        var arrowHead = new Polygon
        {
            Fill = AppConstants.s_DefaultEdgeBrush,
            Stroke = AppConstants.s_DefaultEdgeBrush,
            StrokeThickness = 1,
            Tag = edge
        };
        Panel.SetZIndex(arrowHead, 1);
        UpdateArrowHeadPoints(arrowHead, edge);

        // Подписка на изменения координат.
        edge.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName is nameof(GraphEdge.X1) or nameof(GraphEdge.Y1) or
                                     nameof(GraphEdge.X2) or nameof(GraphEdge.Y2))
            {
                mainLine.X1 = edge.X1; mainLine.Y1 = edge.Y1;
                mainLine.X2 = edge.X2; mainLine.Y2 = edge.Y2;
                hitTestLine.X1 = edge.X1; hitTestLine.Y1 = edge.Y1;
                hitTestLine.X2 = edge.X2; hitTestLine.Y2 = edge.Y2;
                UpdatePredicatePosition(predicateText, edge);
                UpdateArrowHeadPoints(arrowHead, edge);
            }
            else if (args.PropertyName == nameof(GraphEdge.Predicate))
            {
                predicateText.Text = edge.Predicate.ToString();
            }
        };

        return new EdgeVisuals
        {
            Edge = edge,
            MainLine = mainLine,
            HitTestLine = hitTestLine,
            PredicateText = predicateText,
            ArrowHead = arrowHead
        };
    }

    private static void UpdatePredicatePosition(TextBlock text, GraphEdge edge)
    {
        double midX = (edge.X1 + edge.X2) / 2;
        double midY = (edge.Y1 + edge.Y2) / 2;
        Canvas.SetLeft(text, midX);
        Canvas.SetTop(text, midY);
    }
    
    private static void UpdateArrowHeadPoints(Polygon arrow, GraphEdge edge)
    {
        double dx = edge.X2 - edge.X1;
        double dy = edge.Y2 - edge.Y1;
        double len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 0.001) return;

        double ux = dx / len;
        double uy = dy / len;

        const double arrowLength = 12.0;
        const double arrowWidth  = 6.0;
        
        double bx = edge.X2 - ux * arrowLength;
        double by = edge.Y2 - uy * arrowLength;
        
        double px = -uy * arrowWidth;
        double py =  ux * arrowWidth;

        arrow.Points = new PointCollection
        {
            new Point(edge.X2, edge.Y2),       
            new Point(bx + px, by + py),
            new Point(bx - px, by - py)
        };
    }

    /// <summary>
    /// Контейнер для всех визуальных элементов одной дуги.
    /// Все свойства публичные для доступа из GraphRenderer.
    /// </summary>
    public class EdgeVisuals
    {
        public GraphEdge Edge { get; set; } = null!;
        public Line MainLine { get; set; } = null!;
        public Line HitTestLine { get; set; } = null!;
        public TextBlock PredicateText { get; set; } = null!;
        public Polygon ArrowHead { get; set; } = null!;
    }
}
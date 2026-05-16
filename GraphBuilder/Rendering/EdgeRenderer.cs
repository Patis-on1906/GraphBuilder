using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphBuilder.Constants;
using GraphBuilder.Models;

namespace GraphBuilder.Rendering;

public static class EdgeRenderer
{
    /// <summary>
    /// Создаёт все визуальные элементы дуги и возвращает контейнер для них.
    /// </summary>
    public static EdgeVisuals Create(GraphEdge edge)
    {
        // Основная видимая линия
        var mainLine = new Line
        {
            X1 = edge.X1, Y1 = edge.Y1,
            X2 = edge.X2, Y2 = edge.Y2,
            Stroke = AppConstants.DefaultEdgeBrush,
            StrokeThickness = 2,
            Tag = edge
        };

        // Прозрачная линия для удобного hit-test (толще, чтобы легче попасть)
        var hitTestLine = new Line
        {
            X1 = edge.X1, Y1 = edge.Y1,
            X2 = edge.X2, Y2 = edge.Y2,
            Stroke = Brushes.Transparent,
            StrokeThickness = AppConstants.HitTestTolerance * 2,
            IsHitTestVisible = true,
            Tag = edge
        };

        // Текст предиката
        var predicateText = new TextBlock
        {
            Text = edge.Predicate.ToString(),
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Background = Brushes.White,
            Padding = new Thickness(2)
        };
        UpdatePredicatePosition(predicateText, edge);

        // Подписка на изменения модели
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
            PredicateText = predicateText
        };
    }

    private static void UpdatePredicatePosition(TextBlock text, GraphEdge edge)
    {
        double midX = (edge.X1 + edge.X2) / 2;
        double midY = (edge.Y1 + edge.Y2) / 2;
        Canvas.SetLeft(text, midX);
        Canvas.SetTop(text, midY);
    }

    public static void Highlight(EdgeVisuals visuals, bool isHighlighted)
    {
        if (visuals?.MainLine != null)
        {
            visuals.MainLine.Stroke = isHighlighted 
                ? AppConstants.HighlightEdgeBrush 
                : AppConstants.DefaultEdgeBrush;
            visuals.MainLine.StrokeThickness = isHighlighted ? 4 : 2;
        }
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
    }
}
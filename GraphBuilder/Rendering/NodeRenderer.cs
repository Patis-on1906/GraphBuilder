using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphBuilder.Constants;
using GraphBuilder.Models;

namespace GraphBuilder.Rendering;

public static class NodeRenderer
{
    public static Grid Create(GraphNode node)
    {
        var container = new Grid
        {
            Width = node.Radius * 2,
            Height = node.Radius * 2,
            Tag = node // Сохраняем ссылку на модель
        };

        var circle = new Ellipse
        {
            Fill = AppConstants.DefaultNodeBrush,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        var textBlock = new TextBlock
        {
            Text = node.Id.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = FontWeights.Bold,
            FontSize = 14
        };

        container.Children.Add(circle);
        container.Children.Add(textBlock);

        // Начальная позиция
        UpdatePosition(container, node);

        // Подписка на изменения модели
        node.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName is nameof(GraphNode.X) or nameof(GraphNode.Y) or nameof(GraphNode.Radius))
            {
                UpdatePosition(container, node);
                container.Width = node.Radius * 2;
                container.Height = node.Radius * 2;
                circle.Width = node.Radius * 2;
                circle.Height = node.Radius * 2;
            }
        };

        return container;
    }

    private static void UpdatePosition(UIElement container, GraphNode node)
    {
        // Canvas.Left/Top задают позицию ВЕРХНЕГО ЛЕВОГО угла
        // Вычитаем радиус, чтобы центр окружности был в (X, Y)
        Canvas.SetLeft(container, node.X - node.Radius);
        Canvas.SetTop(container, node.Y - node.Radius);
    }

    public static void Highlight(Grid container, bool isHighlighted)
    {
        if (container?.Children[0] is Ellipse circle)
        {
            circle.Fill = isHighlighted 
                ? AppConstants.HighlightNodeBrush 
                : AppConstants.DefaultNodeBrush;
        }
    }
}
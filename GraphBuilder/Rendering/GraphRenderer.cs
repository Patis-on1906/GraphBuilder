using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GraphBuilder.Models;

namespace GraphBuilder.Rendering;

public class GraphRenderer
{
    private readonly Canvas _canvas;
    private readonly Dictionary<int, Grid> _nodeViews = new();
    private readonly Dictionary<int, EdgeRenderer.EdgeVisuals> _edgeViews = new();

    public GraphRenderer(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void Clear()
    {
        _canvas.Children.Clear();
        _nodeViews.Clear();
        _edgeViews.Clear();
    }

    public void RenderAll(Graph graph)
    {
        Clear();

        // Сначала дуги (чтобы были под узлами)
        foreach (var node in graph.Nodes)
        {
            foreach (var edge in node.OutgoingEdges)
            {
                AddEdge(edge);
            }
        }

        // Затем узлы (поверх дуг)
        foreach (var node in graph.Nodes)
        {
            AddNode(node);
        }
    }

    public void AddNode(GraphNode node)
    {
        var uiElement = NodeRenderer.Create(node);
        _canvas.Children.Add(uiElement);
        _nodeViews[node.Id] = uiElement;
    }

    public void AddEdge(GraphEdge edge)
    {
        var visuals = EdgeRenderer.Create(edge);
        
        // Добавляем на Canvas в правильном порядке (Z-индекс)
        _canvas.Children.Add(visuals.HitTestLine);   // Прозрачная, для кликов
        _canvas.Children.Add(visuals.MainLine);        // Видимая линия
        _canvas.Children.Add(visuals.PredicateText);   // Текст
        
        _edgeViews[edge.AbsoluteId] = visuals;
    }

    public void RemoveNode(int nodeId)
    {
        if (_nodeViews.TryGetValue(nodeId, out var uiElement))
        {
            _canvas.Children.Remove(uiElement);
            _nodeViews.Remove(nodeId);
        }
    }

    public void RemoveEdge(int edgeAbsoluteId)
    {
        if (_edgeViews.TryGetValue(edgeAbsoluteId, out var visuals))
        {
            _canvas.Children.Remove(visuals.HitTestLine);
            _canvas.Children.Remove(visuals.MainLine);
            _canvas.Children.Remove(visuals.PredicateText);
            _edgeViews.Remove(edgeAbsoluteId);
        }
    }

    public GraphNode? HitTestNode(Point mousePos)
    {
        foreach (var kvp in _nodeViews)
        {
            var node = kvp.Value.Tag as GraphNode;
            if (node == null) continue;
            
            double dx = mousePos.X - node.X;
            double dy = mousePos.Y - node.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            
            if (distance <= node.Radius + Constants.AppConstants.HitTestTolerance)
            {
                return node;
            }
        }
        return null;
    }

    public GraphEdge? HitTestEdge(Point mousePos)
    {
        foreach (var kvp in _edgeViews)
        {
            var visuals = kvp.Value;
            if (IsPointNearLineSegment(mousePos, 
                new Point(visuals.Edge.X1, visuals.Edge.Y1), 
                new Point(visuals.Edge.X2, visuals.Edge.Y2), 
                Constants.AppConstants.HitTestTolerance))
            {
                return visuals.Edge;
            }
        }
        return null;
    }

    private bool IsPointNearLineSegment(Point point, Point lineStart, Point lineEnd, double tolerance)
    {
        double dx = lineEnd.X - lineStart.X;
        double dy = lineEnd.Y - lineStart.Y;
        
        if (dx == 0 && dy == 0)
        {
            return Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2)) <= tolerance;
        }
        
        double t = ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);
        t = Math.Max(0, Math.Min(1, t));
        
        double closestX = lineStart.X + t * dx;
        double closestY = lineStart.Y + t * dy;
        
        double distance = Math.Sqrt(Math.Pow(point.X - closestX, 2) + Math.Pow(point.Y - closestY, 2));
        return distance <= tolerance;
    }

    // Методы для анимации
    public void HighlightNode(int nodeId, bool isHighlighted)
    {
        if (_nodeViews.TryGetValue(nodeId, out var container))
        {
            NodeRenderer.Highlight(container, isHighlighted);
        }
    }

    public void HighlightEdge(int edgeId, bool isHighlighted)
    {
        if (_edgeViews.TryGetValue(edgeId, out var visuals))
        {
            EdgeRenderer.Highlight(visuals, isHighlighted);
        }
    }

    public Grid? GetNodeView(int nodeId) => _nodeViews.TryGetValue(nodeId, out var view) ? view : null;
    public EdgeRenderer.EdgeVisuals? GetEdgeView(int edgeId) => _edgeViews.TryGetValue(edgeId, out var view) ? view : null;
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphBuilder.Interfaces;
using GraphBuilder.Models;
using GraphBuilder.Rendering;

namespace GraphBuilder.Editor;

public class GraphEditorController
{
    private readonly IGraphAnimationView _view;
    private readonly Canvas _canvas;
    private readonly Graph _graph;
    private readonly IAnimationService _animationService;
    private readonly GraphRenderer _renderer;

    public bool IsEditing { get; private set; } = true;
    public bool IsAnimating => _animationService?.IsRunning == true;

    public GraphEditorController(IGraphAnimationView view, Canvas canvas, Graph graph, IAnimationService animationService)
    {
        _view = view;
        _canvas = canvas;
        _graph = graph;
        _animationService = animationService;
        _renderer = new GraphRenderer(_canvas);
    
        _canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
        _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        _canvas.MouseMove += Canvas_MouseMove;
        _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
    }


    public void CreateNewGraph()
    {
        _graph.Clear();
        _renderer.RenderAll(_graph);
        IsEditing = true;
    }

    public void AddNodeAt(double x, double y)
    {
        var node = _graph.AddNode(x, y);
        _renderer.AddNode(node); // ✅ Метод называется AddNode, а не AddNodeToCanvas
    }

    public void StartAnimation(double durationSeconds)
    {
        if (!IsEditing) return;
        if (_graph.Nodes.Count == 0)
        {
            _view.ShowAnimationError("Граф пуст.");
            return;
        }
        if (_graph.GetNode(1) == null)
        {
            _view.ShowAnimationError("Для анимации необходим узел с номером 1.");
            return;
        }
        IsEditing = false;
        _animationService.Start(_graph, _view, durationSeconds);
    }

    public void StopAnimation()
    {
        _animationService?.Stop();
        IsEditing = true;
        foreach (var node in _graph.Nodes)
        {
            _renderer.HighlightNode(node.Id, false);
        }
    }

    // === Обработчики мыши ===

    private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsEditing) return;
        var pos = e.GetPosition(_canvas);
        AddNodeAt(pos.X, pos.Y);
        e.Handled = true;
    }

    private Point _dragStart;
    private GraphNode? _draggedNode;

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsEditing) return;
    
        var pos = e.GetPosition(_canvas);
        
        if (e.ClickCount == 2)
        {
            HandleDoubleClick(pos);
            e.Handled = true;
            return; // Прерываем, чтобы не сработал одинарный клик
        }

        // Одинарный клик: начало перетаскивания узла
        _draggedNode = _renderer.HitTestNode(pos);
        if (_draggedNode != null)
        {
            _dragStart = pos;
            _canvas.CaptureMouse();
            e.Handled = true;
        }
    }

// Новый метод для обработки двойного клика:
    private void HandleDoubleClick(Point pos)
    {
        var node = _renderer.HitTestNode(pos);
        if (node != null)
        {
            _view.ShowAnimationError($"Двойной клик по узлу {node.Id}");
            return;
        }

        var edge = _renderer.HitTestEdge(pos);
        if (edge != null)
        {
            _view.ShowAnimationError($"Двойной клик по дуге {edge.AbsoluteId}");
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!IsEditing || _draggedNode == null) return;
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var pos = e.GetPosition(_canvas);
            _draggedNode.X += pos.X - _dragStart.X;
            _draggedNode.Y += pos.Y - _dragStart.Y;
            _dragStart = pos;
            // Здесь позже добавим обновление координат дуг
        }
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_draggedNode != null)
        {
            _canvas.ReleaseMouseCapture();
            _draggedNode = null;
            e.Handled = true;
        }
    }
}
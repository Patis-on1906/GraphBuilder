using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphBuilder.Interfaces;
using GraphBuilder.Models;
using GraphBuilder.Rendering;
using GraphBuilder.Services;
using GraphBuilder.Views;

namespace GraphBuilder.Editor;

/// <summary>
/// Центральный контроллер, координирующий работу редактора.
/// Обрабатывает события мыши (добавление, перемещение, удаление узлов, создание дуг),
/// управляет режимами редактирования/анимации, связывает сервисы и рендеринг.
/// </summary>
public class GraphEditorController
{
    private readonly IGraphAnimationView _view;
    private readonly Canvas _canvas;
    private readonly Graph _graph;
    private readonly IAnimationService _animationService;
    private readonly GraphService  _graphService;
    private readonly GraphRenderer _renderer;
    private readonly EdgeCreationHandler _edgeCreator;
    private Point _dragStart;
    private GraphNode? _draggedNode;
    
    public GraphRenderer Renderer => _renderer;
    public bool IsEditing { get; set; } = true;

    public GraphEditorController(IGraphAnimationView view, Canvas canvas, Graph graph, IAnimationService animationService)
    {
        _view = view;
        _canvas = canvas;
        _graph = graph;
        _animationService = animationService;
        _graphService = new GraphService(_graph);
        _renderer = new GraphRenderer(_canvas);

        _graphService.RebuildIndex();
        
        _edgeCreator = new EdgeCreationHandler(_canvas, _graphService, _renderer);
    
        _canvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
        _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        _canvas.MouseMove += Canvas_MouseMove;
        _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
    }

    public void RebuildGraphService() => _graphService?.RebuildIndex();

    public void CreateNewGraph()
    {
        _graph.Clear();
        _graphService.RebuildIndex();
        _renderer.RenderAll(_graph);
        IsEditing = true;
    }

    public void AddNodeAt(double x, double y)
    {
        var node = _graphService.AddNode(x, y);
        _renderer.AddNode(node);
    }

    /// <summary>
    /// Запускает анимацию, если граф не пуст и существует узел 1.
    /// Отключает режим редактирования и блокирует создание дуг.
    /// </summary>
    /// <param name="durationSeconds">Длительность анимации в секундах.</param>
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
        _edgeCreator.Disable(); 
    }

    public void StopAnimation()
    {
        _animationService?.Stop();
        IsEditing = true;
        foreach (var node in _graph.Nodes)
            _renderer.HighlightNode(node.Id, false);
        _edgeCreator.Enable();
        
        _canvas.Focus();
    }
    
    private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsEditing) return;

        var pos = e.GetPosition(_canvas);
        var clickedNode = _renderer.HitTestNode(pos);

        if (clickedNode != null)
        {
            if (clickedNode.Id == 1)
            {
                MessageBox.Show("Узел с номером 1 является стартовым и не может быть удалён.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
                return;
            }

            _graphService.RemoveNode(clickedNode.Id);
            _renderer.RemoveNode(clickedNode.Id);
            _renderer.RefreshEdges(_graph);
        }
        else
        {
            AddNodeAt(pos.X, pos.Y);
        }

        e.Handled = true;
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsEditing) return;
    
        var pos = e.GetPosition(_canvas);
        
        if (e.ClickCount == 2)
        {
            HandleDoubleClick(pos);
            e.Handled = true;
            return;
        }

        // Одинарный клик: начало перетаскивания узла.
        _draggedNode = _renderer.HitTestNode(pos);
        if (_draggedNode != null)
        {
            _dragStart = pos;
            _canvas.CaptureMouse();
            e.Handled = true;
        }
    }

    private void HandleDoubleClick(Point pos)
    {
        var node = _renderer.HitTestNode(pos);
        if (node != null)
        {
            var dialog = new NodeCodeDialog(node.Code) { Owner = Application.Current.MainWindow };
            if (dialog.ShowDialog() == true && dialog.ResultCode != null)
                node.Code = dialog.ResultCode;
            return;
        }

        var edge = _renderer.HitTestEdge(pos);
        if (edge != null)
        {
            var sourceNode = _graph.GetNode(edge.SourceNodeId);
            int maxPredicate = sourceNode?.OutgoingEdges.Count ?? 1;
            
            var edgeDialog = new EdgeDialog(edge.Predicate, edge.DelaySeconds, maxPredicate)
                { Owner = Application.Current.MainWindow };

            if (edgeDialog.ShowDialog() == true)
            {
                edge.Predicate = edgeDialog.ResultPredicate ?? edge.Predicate;
                edge.DelaySeconds = edgeDialog.ResultDelay ?? edge.DelaySeconds;
            }
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
            
            _graphService?.UpdateEdgeCoordinates(_draggedNode.Id);
        }
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_draggedNode != null)
        {
            _canvas.ReleaseMouseCapture();
            _draggedNode = null;
            _renderer.RefreshEdges(_graph);

            e.Handled = true;
        }
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphBuilder.Constants;
using GraphBuilder.Models;
using GraphBuilder.Rendering;
using GraphBuilder.Services;

namespace GraphBuilder.Editor;

public class EdgeCreationHandler
{
    private readonly Canvas _canvas;
    private readonly GraphService _graphService;
    private readonly GraphRenderer _renderer;
    
    private GraphNode? _startNode;
    private Point _startBoundaryPoint;
    private Line? _tempLine;
    private bool _isCreating;
    private bool _isEnabled = true;

    public EdgeCreationHandler(Canvas canvas, GraphService graphService, GraphRenderer renderer)
    {
        _canvas = canvas;
        _graphService = graphService;
        _renderer = renderer;
        
        _canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        _canvas.MouseMove += Canvas_MouseMove;
        _canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_isEnabled) return;
        
        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl)) 
            return;
    
        var pos = e.GetPosition(_canvas);
        _startNode = _renderer.HitTestNode(pos);
    
        if (_startNode != null)
        {
            _startBoundaryPoint = CalculateBoundaryPoint(_startNode, pos);
            StartCreation(pos);
            e.Handled = true;
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isEnabled || !_isCreating || _tempLine == null) return;
        
        var pos = e.GetPosition(_canvas);
        _tempLine.X2 = pos.X;
        _tempLine.Y2 = pos.Y;
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isEnabled || !_isCreating) return;
        
        var pos = e.GetPosition(_canvas);
        var targetNode = _renderer.HitTestNode(pos);
        
        if (targetNode != null && targetNode.Id != _startNode?.Id)
        {
            FinishCreation(targetNode, pos);
        }
        else
        {
            CancelCreation();
        }
        
        e.Handled = true;
    }

    private void StartCreation(Point startPos)
    {
        _isCreating = true;
        
        _tempLine = new Line
        {
            X1 = _startBoundaryPoint.X,
            Y1 = _startBoundaryPoint.Y,
            X2 = startPos.X,
            Y2 = startPos.Y,
            Stroke = Brushes.Gray,
            StrokeThickness = 2,
            StrokeDashArray = new DoubleCollection { 4, 2 }
        };
        _canvas.Children.Add(_tempLine);
    }

    private void FinishCreation(GraphNode targetNode, Point endPos)
    {
        var endBoundaryPoint = CalculateBoundaryPoint(targetNode, endPos);
        var newEdge = _graphService.AddEdge(
            _startNode!.Id, 
            targetNode.Id, 
            _startBoundaryPoint.X, _startBoundaryPoint.Y,
            endBoundaryPoint.X, endBoundaryPoint.Y);
        _renderer.AddEdge(newEdge);
        RemoveTempLine();
        Cleanup();
    }

    private void CancelCreation()
    {
        RemoveTempLine(); 
        Cleanup();
    }
    
    private void RemoveTempLine()
    {
        if (_tempLine != null)
        {
            _canvas.Children.Remove(_tempLine);
            _tempLine = null;
        }
    }

    private void Cleanup()
    {
        _isCreating = false;
        _startNode = null;
    }
    
    // Вычисляет точку на окружности узла, ближайшую к заданной точке.
    private Point CalculateBoundaryPoint(GraphNode node, Point externalPoint)
    {
        double dx = externalPoint.X - node.X;
        double dy = externalPoint.Y - node.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        
        // Если курсор в центре узла — возвращаем верхнюю точку окружности
        if (distance < 0.001)
            return new Point(node.X, node.Y - node.Radius);
        
        double ratio = node.Radius / distance;
        return new Point(node.X + dx * ratio, node.Y + dy * ratio);
    }
    
    public void Disable()
    {
        _isEnabled = false;
        RemoveTempLine();      
        CancelCreation();      
    }

    public void Enable()
    {
        _isEnabled = true;
    }
}
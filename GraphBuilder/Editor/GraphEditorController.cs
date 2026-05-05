using System.Windows.Controls;
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
    public bool IsAnimating => _animationService.IsRunning;

    public GraphEditorController(IGraphAnimationView view, Canvas canvas, Graph graph,
        IAnimationService animationService)
    {
        _view = view;
        _canvas = canvas;
        _graph = graph;
        _animationService = animationService;
        _renderer = new GraphRenderer(_canvas);
    }

    public void CreateNewGraph()
    {
        _graph.Clear();
        _renderer.Clear();
        IsEditing = true;
    }

    public void StartAnimation(double durationSeconds)
    {
        if (!IsEditing) return;

        if (_graph.Nodes.Count == 0)
        {
            _view.ShowAnimationError("Граф пуст. Добавьте хотя бы один узел.");
            return;
        }
        
        IsEditing = false;
        _animationService.Start(_graph, _view, durationSeconds);
    }

    public void StopAnimation()
    {
        if (IsAnimating)
        {
            _animationService.Stop();
            IsEditing = true;
            _view.UnhighlightNode(-1);
        }
    }
}
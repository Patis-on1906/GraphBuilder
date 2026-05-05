using System.Windows.Controls;
namespace GraphBuilder.Rendering;

public class GraphRenderer
{
    private readonly Canvas _canvas;
    public GraphRenderer(Canvas canvas) => _canvas = canvas;
    
    // Очищает холст
    public void Clear() => _canvas.Children.Clear();
}
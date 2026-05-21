using System.Windows.Media;

namespace GraphBuilder.Constants;

public static class AppConstants
{
    // Кисти для отрисовки
    public static readonly Brush DefaultNodeBrush = Brushes.LightGray;
    public static readonly Brush HighlightNodeBrush = Brushes.LightGreen;
    public static readonly Brush DefaultEdgeBrush = Brushes.Black;
    public static readonly Brush HighlightEdgeBrush = Brushes.Red;
    
    public const double HitTestTolerance = 5.0; // Допуск в пикселях для клика по границе узла/дуги
    public const string DefaultNodeCode = "random(1,N)";
}
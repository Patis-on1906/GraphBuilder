using System.Windows.Media;

namespace GraphBuilder.Constants;

public static class AppConstants
{
    // Кисти для отрисовки.
    public static readonly Brush s_DefaultNodeBrush = Brushes.LightGray;
    public static readonly Brush s_HighlightNodeBrush = Brushes.LightGreen;
    public static readonly Brush s_DefaultEdgeBrush = Brushes.Black;
    
    // Допуск в пикселях для клика по границе узла/дуги.
    public const double HitTestTolerance = 5.0;
    
    public const string DefaultNodeCode = "random(1,N)";
}
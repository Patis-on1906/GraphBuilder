using System.Windows.Media;

namespace GraphBuilder.Constants;

public static class AppConstants
{
    // Кисти для отрисовки
    public static readonly Brush DefaultNodeBrush = Brushes.LightGray;
    public static readonly Brush HighlightNodeBrush = Brushes.LightGreen;
    public static readonly Brush SelectedNodeBrush = Brushes.LightSkyBlue;
    
    public static readonly Brush DefaultEdgeBrush = Brushes.Black;
    public static readonly Brush HighlightEdgeBrush = Brushes.Red;
    public static readonly Pen DefaultEdgePen = new(DefaultEdgeBrush, 2);
    public static readonly Pen HighlightEdgePen = new(HighlightEdgeBrush, 3);

    // Геометрия и взаимодействие
    public const double DefaultNodeRadius = 30.0;
    public const double HitTestTolerance = 5.0; // Допуск в пикселях для клика по границе узла/дуги

    // Анимация и данные
    public const double DefaultDelaySeconds = 2.0;
    public const string DefaultNodeCode = "GEN_RANDOM(1,N)";
    public const int StartNodeId = 1;
}
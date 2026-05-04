using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GraphBuilder.Models;

public class GraphEdge : INotifyPropertyChanged
{
    public int AbsoluteId { get; init; }
    public int LocalId { get; init; }
    public int TargetNodeId { get; set; }

    // Координаты начальной и конечной точек
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }

    public int Predicate { get; set; }
    public double DelaySeconds { get; set; } = 2.0;

    public GraphEdge(int absoluteId, int localId, int targetId, double x1, double y1, double x2, double y2)
    {
        AbsoluteId = absoluteId;
        LocalId = localId;
        TargetNodeId = targetId;
        X1 = x1; Y1 = y1; X2 = x2; Y2 = y2;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
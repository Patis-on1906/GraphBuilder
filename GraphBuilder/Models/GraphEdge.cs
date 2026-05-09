using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GraphBuilder.Models;

public class GraphEdge : INotifyPropertyChanged
{
    public int AbsoluteId { get; init; }
    public int LocalId { get; init; }

    private int _targetNodeId;
    public int TargetNodeId
    {
        get => _targetNodeId;
        set
        {
            if (_targetNodeId != value)
            {
                _targetNodeId = value;
                OnPropertyChanged();
            }
        }
    }

    private double _x1;
    public double X1
    {
        get => _x1;
        set { if (Math.Abs(_x1 - value) > 0.001) { _x1 = value; OnPropertyChanged(); } }
    }

    private double _y1;
    public double Y1
    {
        get => _y1;
        set { if (Math.Abs(_y1 - value) > 0.001) { _y1 = value; OnPropertyChanged(); } }
    }

    private double _x2;
    public double X2
    {
        get => _x2;
        set { if (Math.Abs(_x2 - value) > 0.001) { _x2 = value; OnPropertyChanged(); } }
    }

    private double _y2;
    public double Y2
    {
        get => _y2;
        set { if (Math.Abs(_y2 - value) > 0.001) { _y2 = value; OnPropertyChanged(); } }
    }

    private int _predicate;
    public int Predicate
    {
        get => _predicate;
        set { if (_predicate != value) { _predicate = value; OnPropertyChanged(); } }
    }

    private double _delaySeconds = 2.0;
    public double DelaySeconds
    {
        get => _delaySeconds;
        set { if (Math.Abs(_delaySeconds - value) > 0.001) { _delaySeconds = value; OnPropertyChanged(); } }
    }

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
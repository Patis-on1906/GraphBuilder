using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GraphBuilder.Models;

public class GraphNode : INotifyPropertyChanged
{
    public int Id { get; init; }
    public ObservableCollection<GraphEdge> OutgoingEdges { get; } = new();

    private double _x;
    public double X
    {
        get => _x;
        set
        {
            if (Math.Abs(_x - value) > 0.001)
            {
                _x = value;
                OnPropertyChanged();
            }
        }
    }

    private double _y;
    public double Y
    {
        get => _y;
        set
        {
            if (Math.Abs(_y - value) > 0.001)
            {
                _y = value;
                OnPropertyChanged();
            }
        }
    }

    private double _radius = 30.0;
    public double Radius
    {
        get => _radius;
        set
        {
            if (Math.Abs(_radius - value) > 0.001)
            {
                _radius = value;
                OnPropertyChanged();
            }
        }
    }

    private string _code = string.Empty;  // ? Символы, идентифицирующие состояния ?
    public string Code
    {
        get => _code;
        set
        {
            if (_code != value)
            {
                _code = value;
                OnPropertyChanged();
            }
        }
    }

    public GraphNode(int id, double x, double y)
    {
        Id = id;
        X = x;
        Y = y;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
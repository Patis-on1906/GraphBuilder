using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace GraphBuilder.Models;

/// <summary>
/// Представляет вершину (состояние) графа.
/// Содержит координаты, радиус, программный код и список исходящих дуг.
/// </summary>
public class GraphNode : INotifyPropertyChanged
{
    [XmlAttribute("Id")]
    public int Id { get; init; }

    [XmlArray("OutgoingEdges")]
    [XmlArrayItem("OutgoingEdge")]
    public ObservableCollection<GraphEdge> OutgoingEdges { get; } = new();

    private double _x;
    [XmlAttribute("X")]
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
    [XmlAttribute("Y")]
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
    [XmlAttribute("Radius")]
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

    private string _code = string.Empty;
    [XmlAttribute("Code")]
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

    public GraphNode() { }
}
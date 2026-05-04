using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GraphBuilder.Models;

public class GraphNode : INotifyPropertyChanged
{
    public int Id { get; init; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius { get; set; } = 30.0;
    public string Code { get; set; } = string.Empty;
    
    public ObservableCollection<GraphEdge> OutgoingEdges { get; } = new();

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
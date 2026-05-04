using System.Collections.ObjectModel;

namespace GraphBuilder.Models;

public class Graph
{
    public ObservableCollection<GraphNode> Nodes { get; } = new();
    
    // Счётчики для генерации уникальных ID
    public int NextNodeId { get; private set; } = 1;
    public int NextEdgeAbsoluteId { get; private set; } = 1;

    public GraphNode AddNode(double x, double y)
    {
        var node = new GraphNode(NextNodeId++, x, y);
        Nodes.Add(node);
        return node;
    }

    public GraphEdge AddEdge(int sourceNodeId, int targetNodeId, double x1, double y1, double x2, double y2)
    {
        var source = Nodes.First(n => n.Id == sourceNodeId);
        var edge = new GraphEdge(NextEdgeAbsoluteId++, source.OutgoingEdges.Count + 1, targetNodeId, x1, y1, x2, y2);
        source.OutgoingEdges.Add(edge);
        return edge;
    }

    public GraphNode? GetNode(int id) => Nodes.FirstOrDefault(n => n.Id == id);

    public void Clear()
    {
        Nodes.Clear();
        NextNodeId = 1;
        NextEdgeAbsoluteId = 1;
    }
}
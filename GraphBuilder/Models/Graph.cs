using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace GraphBuilder.Models;

[XmlRoot("Graph")]
public class Graph
{
    [XmlArray("Nodes")]
    [XmlArrayItem("Node")]
    public ObservableCollection<GraphNode> Nodes { get; } = new();
    
    [XmlAttribute("NextNodeID")]
    public int NextNodeId { get; set; } = 1;
    [XmlAttribute("NextEdgeAbsId")]
    public int NextEdgeAbsoluteId { get; set; } = 1;

    public GraphNode AddNode(double x, double y)
    {
        var node = new GraphNode(NextNodeId++, x, y)
        {
            Code = Constants.AppConstants.DefaultNodeCode
        };
        Nodes.Add(node);
        return node;
    }

    public GraphEdge AddEdge(int sourceNodeId, int targetNodeId, double x1, double y1, double x2, double y2)
    {
        var source = Nodes.First(n => n.Id == sourceNodeId);
        int localId = source.OutgoingEdges.Count + 1;
        var edge = new GraphEdge(
            NextEdgeAbsoluteId++, 
            localId, 
            sourceNodeId,
            targetNodeId, 
            x1, y1, x2, y2)
        {
            Predicate = localId
        };
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

    public Graph() { }

    /// <summary>
    /// Копирует данные из другого графа в текущий экземпляр.
    /// Используется при загрузке из XML.
    /// </summary>
    public void CopyFrom(Graph source)
    {
        Clear();
    
        // Копируем счётчики
        NextNodeId = source.NextNodeId;
        NextEdgeAbsoluteId = source.NextEdgeAbsoluteId;
    
        // Копируем узлы и их дуги
        foreach (var sourceNode in source.Nodes)
        {
            var newNode = new GraphNode(sourceNode.Id, sourceNode.X, sourceNode.Y)
            {
                Radius = sourceNode.Radius,
                Code = sourceNode.Code
            };
            Nodes.Add(newNode);
            
            foreach (var sourceEdge in sourceNode.OutgoingEdges)
            {
                var newEdge = new GraphEdge(
                    sourceEdge.AbsoluteId,
                    sourceEdge.LocalId,
                    sourceEdge.SourceNodeId, 
                    sourceEdge.TargetNodeId,
                    sourceEdge.X1, sourceEdge.Y1,
                    sourceEdge.X2, sourceEdge.Y2)
                {
                    Predicate = sourceEdge.Predicate,
                    DelaySeconds = sourceEdge.DelaySeconds
                };
                newNode.OutgoingEdges.Add(newEdge);
            }
        }
    }
}
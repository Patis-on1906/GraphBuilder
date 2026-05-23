using System;
using System.Collections.Generic;
using System.Linq;
using GraphBuilder.Models;

namespace GraphBuilder.Services;

public class GraphService
{
    private readonly Graph _graph;
    private Dictionary<int, GraphNode> _nodeById = new();

    public GraphService(Graph graph)
    { 
        _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        RebuildIndex();
    }
    
    public void RebuildIndex()
    {
        _nodeById = _graph.Nodes.ToDictionary(n => n.Id);
    }

    private GraphNode? GetNode(int id) => 
        _nodeById.TryGetValue(id, out var node) ? node : null;

    public GraphNode AddNode(double x, double y)
    {
        var newNode = _graph.AddNode(x, y);
        _nodeById[newNode.Id] = newNode;
        return newNode;                 
    }
    
    public GraphEdge AddEdge(int srcId, int trgId, double x1, double y1, double x2, double y2)
    {
        if (srcId == trgId && (Math.Abs(x1 - x2) < 0.001 && Math.Abs(y1 - y2) < 0.001))
        {
            throw new ArgumentException("Дуга не может быть вырожденной в точку");
        }
    
        return _graph.AddEdge(srcId, trgId, x1, y1, x2, y2);
    }

    public void RemoveNode(int id)
    {
        var node = GetNode(id);
        if (node == null)
        {
            throw new ArgumentException($"Узел с Id={id} не существует");
        }
        
        node.OutgoingEdges.Clear();

        // Удаляем все входящие дуги из других узлов.
        foreach (var otherNode in _graph.Nodes.ToList()) 
        {
            for (int i = otherNode.OutgoingEdges.Count - 1; i >= 0; i--)
            {
                if (otherNode.OutgoingEdges[i].TargetNodeId == id)
                {
                    otherNode.OutgoingEdges.RemoveAt(i);
                }
            }
        }
        
        _graph.Nodes.Remove(node);
        _nodeById.Remove(id);
    }
    
    public void UpdateEdgeCoordinates(int nodeId)
    {
        var node = GetNode(nodeId);
        if (node == null) return;
        
        foreach (var edge in node.OutgoingEdges)
        {
            var targetNode = GetNode(edge.TargetNodeId);
            if (targetNode != null)
                UpdateEdgeEndpoints(node, targetNode, edge);
        }

        // Обновляем входящие дуги.
        foreach (var otherNode in _graph.Nodes)
        {
            if (otherNode.Id == nodeId) continue;
            foreach (var edge in otherNode.OutgoingEdges)
            {
                if (edge.TargetNodeId == nodeId)
                {
                    var sourceNode = otherNode;
                    var targetNode = node;
                    UpdateEdgeEndpoints(sourceNode, targetNode, edge);
                }
            }
        }
    }
    
    private void UpdateEdgeEndpoints(GraphNode source, GraphNode target, GraphEdge edge)
    {
        double dx = target.X - source.X;
        double dy = target.Y - source.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        
        if (distance < 1e-6)
        {
            UpdateLoopEdgeCoordinates(source, edge);
            return;
        }
        
        double startX = source.X + dx * (source.Radius / distance);
        double startY = source.Y + dy * (source.Radius / distance);
        double endX = target.X - dx * (target.Radius / distance);
        double endY = target.Y - dy * (target.Radius / distance);

        edge.X1 = startX;
        edge.Y1 = startY;
        edge.X2 = endX;
        edge.Y2 = endY;
    }
    
    private void UpdateLoopEdgeCoordinates(GraphNode node, GraphEdge edge)
    {
        edge.X1 = node.X;
        edge.Y1 = node.Y - node.Radius;
        edge.X2 = node.X;
        edge.Y2 = node.Y + node.Radius;
    }
}
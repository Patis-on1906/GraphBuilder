using GraphBuilder.Models;
using GraphBuilder.Services;

namespace Services.Test;

public class ServiceTests
{
    // ТЕСТЫ ДЛЯ GRAPG SERVICE
    private GraphService GetGraphService()
    {
        var graph = new Graph();
        return new GraphService(graph);
    }


    [Fact]
    public void AddNode_WhenCalled_AddsNodeToGraph()
    {
        var service = GetGraphService();

        service.AddNode(100, 150);

        var node = service.GetNode(1);
        Assert.NotNull(node);
        Assert.Equal(100, node.X);
        Assert.Equal(150, node.Y);
    }

    [Fact]
    public void RemoveNode_WhenNodeExists_RemovesNodeAndItsEdges()
    {
        var service = GetGraphService();
        service.AddNode(0, 0);      // id = 1
        service.AddNode(100, 100);  // id = 2
        var source_nod = service.GetNode(1);
        var target_nod = service.GetNode(2);
        service.AddEdge(1, 2, source_nod.X, source_nod.Y, target_nod.X, target_nod.Y);
        
        service.RemoveNode(2);

        var removedNode = service.GetNode(2);
        Assert.Null(removedNode);
        // ребро тоже исчезло
        Assert.Empty(source_nod.OutgoingEdges);
    }


    // ТЕСТЫ ДЛЯ VALIDATION SERVICE
    [Fact]
    public void ValidateGraph_ValidGraph_ReturnsTrue()
    {
        var graph = new Graph();
        graph.AddNode(100, 100);
        graph.AddNode(200, 200);
        var node1 = graph.Nodes.First();
        var node2 = graph.Nodes.Last();
        graph.AddEdge(node1.Id, node2.Id, node1.X, node1.Y, node2.X, node2.Y);

        bool isValid = ValidationService.ValidateGraph(graph);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidateGraph_GraphWithNullCode_ReturnsFalse()
    {
        var graph = new Graph();
        var node = graph.AddNode(100, 100);
        node.Code = null; // создание некорректности

        bool isValid = ValidationService.ValidateGraph(graph);

        Assert.False(isValid);
    }
}
using System;

using GraphBuilder.Models;


namespace GraphBuilder.Services
{
    public class GraphService
    {
        private Graph _graph;
        private Dictionary<int, GraphNode> _nodeById;

        public GraphService(Graph graph)
        { 
            _graph = graph;
            _nodeById = new Dictionary<int, GraphNode>();
        }


        public GraphNode GetNode(int id)
        {
            return _nodeById.GetValueOrDefault(id);
        }


        public void AddNode(double x, double y)
        {
            GraphNode new_node = _graph.AddNode(x, y);
            int nod_index = _graph.NextNodeId;
            _nodeById[nod_index] = new_node;
        }


        public void AddEdge(int srcId, int trgId, double x1, double y1, double x2, double y2)
        {
            GraphEdge ge = _graph.AddEdge(srcId, trgId, x1, y1, x2, y2);
        }


        public void RemoveNode(int id)
        {
            bool is_node_exist = _nodeById.ContainsKey(id); // существование узла
            if (!is_node_exist)
            {
                throw new ArgumentException("Узла с таким идентификатором не существует");
            }

            bool is_remove_uspex = _graph.Nodes.Remove(GetNode(id));
            if (!is_remove_uspex)
            {
                throw new ArgumentException("Не удалось удалить узел");
            }
        }


        public void UpdateEdgeCoordinates(GraphNode nodeId)
        {

        }

    }
}
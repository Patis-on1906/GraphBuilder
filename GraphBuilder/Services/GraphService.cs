using System;
using System.Linq;

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
            foreach(GraphNode node in _graph.Nodes)
            {
                _nodeById[node.Id] = node;
            }
        }


        public GraphNode GetNode(int id)
        {
            return _nodeById.GetValueOrDefault(id);
        }


        public void AddNode(double x, double y)
        {
            GraphNode new_nod = _graph.AddNode(x, y);
            _nodeById[new_nod.Id] = new_nod;
        }


        public void AddEdge(int srcId, int trgId, double x1, double y1, double x2, double y2)
        {
            if(srcId == trgId || (x1 == x2 && y1 == y2))
            {
                throw new ArgumentException("Узел никуда не указывает");
            }
            GraphEdge ge = _graph.AddEdge(srcId, trgId, x1, y1, x2, y2);
        }


        public void RemoveNode(int id)
        {
            bool is_node_exist = _nodeById.ContainsKey(id); // существование узла
            if (!is_node_exist)
            {
                throw new ArgumentException("Узла с таким идентификатором не существует");
            }


            GetNode(id).OutgoingEdges.Clear();

            bool is_remove_uspex = _graph.Nodes.Remove(GetNode(id));
            if (!is_remove_uspex)
            {
                throw new ArgumentException("Не удалось удалить узел");
            }

            // Удаление ветвей, у которых этот узел был таргетом
            foreach(GraphNode gn in _graph.Nodes)
            {
                for(int i = gn.OutgoingEdges.Count - 1; i >= 0; i--)
                {
                    if (gn.OutgoingEdges[i].TargetNodeId == id)
                    {
                        gn.OutgoingEdges.RemoveAt(i);
                    }
                }
            }

            _nodeById.Remove(id);
        }


        public void UpdateEdgeCoordinates(int nodeId)
        {
            GraphNode node = GetNode(nodeId);
            foreach(GraphEdge ge in node.OutgoingEdges) // выходящие
            {
                GraphNode cur_trg_node = GetNode(ge.TargetNodeId);
                if(cur_trg_node == null) { continue; }

                if(node != cur_trg_node)    // незамкнутость
                {
                    double dx = cur_trg_node.X - node.X;
                    double dy = cur_trg_node.Y - node.Y;
                    if(Math.Abs(dx) < 0.001) { dx = 0; }
                    if(Math.Abs(dy) < 0.001) { dy = 0; }

                    double distance = Math.Sqrt(dx * dx + dy * dy);
                    if (distance < 1e-6)
                    {
                        if (node.Id == ge.TargetNodeId) { UpdateClosedEdgeCoordinates(nodeId, ge); }
                        continue;
                    }

                    // координаты выходящей дуги - точки на окружности узла
                    double new_x1 = node.X + (dx / distance) * node.Radius;
                    double new_y1 = node.Y + (dy / distance) * node.Radius;

                    ge.X1 = new_x1;
                    ge.Y1 = new_y1;
                }
                else
                {
                    UpdateClosedEdgeCoordinates(nodeId, ge);
                }
            }

            // входящие ветви
            foreach(GraphNode gn in _graph.Nodes)
            {
                if(gn == null) { continue; }
                if(gn.Id == nodeId) { continue; }   // возможно не нужно пропускать, хотя вроде замкнутые уже обработаются на входящем переборе 

                foreach (GraphEdge ge2 in gn.OutgoingEdges)
                {
                    if (ge2.TargetNodeId == nodeId)
                    {
                        double dx = node.X - gn.X;
                        double dy = node.Y - gn.Y;
                        if (Math.Abs(dx) < 0.001) { dx = 0; }
                        if (Math.Abs(dy) < 0.001) { dy = 0; }

                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        if (distance < 1e-6)
                        {
                            if(node.Id == ge2.TargetNodeId) { UpdateClosedEdgeCoordinates(nodeId, ge2); }
                            continue;
                        }

                        double new_x2 = node.X - (dx / distance) * node.Radius;
                        double new_y2 = node.Y - (dy / distance) * node.Radius;

                        ge2.X2 = new_x2;
                        ge2.Y2 = new_y2;
                    }
                }
            }
            
        }


        // из верхней точки окружности в нижнюю: предполагается, что ветви не будут пересекать окружность, а обходить
        private void UpdateClosedEdgeCoordinates(int node_id, GraphEdge edge)
        {
            GraphNode node = GetNode(node_id);
            edge.X1 = node.X;
            edge.X2 = node.X;
            edge.Y1 = node.Y + node.Radius;
            edge.Y2 = node.Y - node.Radius;
        }
    }
}
using GraphBuilder.Models;
using System.Diagnostics;

using System.Linq;


namespace GraphBuilder.Services
{
    public static class ValidationService
    {
        public static bool ValidateGraph(Graph g)   // ? почему координаты не могут быть отрицательнами? Это реализовано, но нелогично. Для ветвей логично условие чтобы начало и конец не были одной точкой
        {
            List<int> absolute_edges_id = new List<int>();

            foreach (GraphNode node in g.Nodes)
            {
                if (node.Code == null) { return false; }

                int[] local_edges_id = new int[node.OutgoingEdges.Count];
                int local_edges_id_couter = 0;
                foreach (GraphEdge edge in node.OutgoingEdges)
                {
                    if(edge.DelaySeconds <= 0) { return false; }

                    local_edges_id[local_edges_id_couter] = edge.LocalId;
                    local_edges_id_couter++;

                    // проверка координат ветви
                    if(edge.X1 < 0 || edge.Y1 < 0 || edge.X2 < 0 || edge.Y2 < 0) { return false; }

                    absolute_edges_id.Add(edge.AbsoluteId);

                    if(!IsTargetNodeExist(g, edge.TargetNodeId)) { return false; }
                }

                for(int i = 0; i < local_edges_id_couter; i++)  // уникальность локальных id ветвей
                {
                    for(int j = 0; j < local_edges_id_couter; j++)
                    {
                        if (local_edges_id[i] == local_edges_id[j] && i != j) { return false; }
                    }
                }


                int equal_id_count = 0;
                foreach(GraphNode node_temp in g.Nodes) // уникальность ID узла
                {
                    if(node.Id == node_temp.Id) { equal_id_count++; }
                }
                if(equal_id_count != 1) { return false; }

                if(node.X < 0 || node.Y < 0) { return false; }  // проверка координат узла
            }

            if(absolute_edges_id.Count != absolute_edges_id.Distinct().Count()) { return false; }   // уникальность AbsoluteId узлов

            return true;
        }


        // проверка существования узла по ID
        public static bool IsTargetNodeExist(Graph graph, int target_node_id)
        {
            bool is_exist = false;
            foreach(GraphNode node in graph.Nodes)
            {
                if(node.Id == target_node_id) { is_exist = true; break; }
            }
            return is_exist;
        }


        // проверка предиката
        public static bool ValidateEdgePredicate(GraphEdge e, int outgoingCount)
        {
            if(e.Predicate >= 1 && e.Predicate <= outgoingCount) { return true; }
            return false;
        }
    }
}
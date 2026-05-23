using GraphBuilder.Models;
using System.Diagnostics;
using System.Linq;

namespace GraphBuilder.Services;

public static class ValidationService
{
    public static bool ValidateGraph(Graph g)   
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

                absolute_edges_id.Add(edge.AbsoluteId);

                if(!IsTargetNodeExist(g, edge.TargetNodeId)) { return false; }
            }

            for(int i = 0; i < local_edges_id_couter; i++)
            {
                for(int j = 0; j < local_edges_id_couter; j++)
                {
                    if (local_edges_id[i] == local_edges_id[j] && i != j) { return false; }
                }
            }

            int equal_id_count = 0;
            foreach(GraphNode node_temp in g.Nodes)
            {
                if(node.Id == node_temp.Id) { equal_id_count++; }
            }
            if(equal_id_count != 1) { return false; }
        }

        if(absolute_edges_id.Count != absolute_edges_id.Distinct().Count()) { return false; }

        return true;
    }
    
    public static bool IsTargetNodeExist(Graph graph, int target_node_id)
    {
        bool is_exist = false;
        foreach(GraphNode node in graph.Nodes)
        {
            if(node.Id == target_node_id) { is_exist = true; break; }
        }
        return is_exist;
    }
}
namespace GraphBuilder.Interfaces;

public interface IGraphAnimationView
{
    void HighlightNode(int nodeId);
    void UnhighlightNode(int nodeId);
    void ShowAnimationError(string message);
    void NotifyAnimationStepCompleted(int fromNodeId, int toNodeId, int edgeId);
}
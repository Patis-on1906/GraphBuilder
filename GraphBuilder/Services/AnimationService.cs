using GraphBuilder.Interfaces;
using GraphBuilder.Models;

namespace GraphBuilder.Services;

public class AnimationService : IAnimationService
{
    public bool IsRunning { get; private set; }

    public void Start(Graph graph, IGraphAnimationView view, double durationSeconds)
    {
        IsRunning = true;
        // Владос заменишь потом на свое
        view.HighlightNode(1);
        view.NotifyAnimationStepCompleted(0, 1, 0);
        System.Threading.Tasks.Task.Run(async () =>
        {
            await System.Threading.Tasks.Task.Delay((int)(durationSeconds * 1000));
            if (IsRunning) Stop();
        });
    }

    public void Stop() => IsRunning = false;
}
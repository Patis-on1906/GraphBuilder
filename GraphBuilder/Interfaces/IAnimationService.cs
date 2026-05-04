using GraphBuilder.Interfaces;
using GraphBuilder.Models;

public interface IAnimationService
{
    /// <param name="graph">Текущий граф</param>
    /// <param name="view">Интерфейс UI для подсветки</param>
    /// <param name="durationSeconds">Общее время анимации</param>
    void Start(Graph graph, IGraphAnimationView view, double durationSeconds);
    
    void Stop();
    bool IsRunning { get; }
}
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphBuilder.Interfaces;
using GraphBuilder.Models;

namespace GraphBuilder.Services
{
    public class AnimationService : IAnimationService
    {
        private CancellationTokenSource _cts;
        private Task _animationTask;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        public void Start(Graph graph, IGraphAnimationView view, double durationSeconds = 3600)
        {
            if (_isRunning) Stop();

            _cts = new CancellationTokenSource();
            _isRunning = true;
            _animationTask = RunAnimation(graph, view, durationSeconds, _cts.Token);
        }

        public void Stop()
        {
            if (!_isRunning) return;
            _cts?.Cancel();
            _isRunning = false;
        }

        private async Task RunAnimation(Graph graph, IGraphAnimationView view,
            double durationSeconds, CancellationToken token)
        {
            bool finishedNormally = false;
            try
            {
                var currentNode = graph.Nodes.FirstOrDefault(n => n.Id == 1);
                if (currentNode == null)
                {
                    view.ShowAnimationError("Стартовый узел (Id=1) не найден в графе");
                    return;
                }

                var startTime = DateTime.UtcNow;
                var random = new Random();

                while (_isRunning && (DateTime.UtcNow - startTime).TotalSeconds < durationSeconds)
                {
                    view.HighlightNode(currentNode.Id);
                    view.NotifyAnimationStepCompleted(-1, currentNode.Id, -1);

                    var outgoingEdges = currentNode.OutgoingEdges;
                    if (outgoingEdges.Count == 0)
                    {
                        view.ShowAnimationError($"Узел {currentNode.Id} не имеет исходящих дуг — анимация остановлена");
                        break;
                    }

                    int selectedPredicate = random.Next(1, outgoingEdges.Count + 1);
                    var selectedEdge = outgoingEdges.FirstOrDefault(e => e.Predicate == selectedPredicate);
                    if (selectedEdge == null)
                    {
                        view.ShowAnimationError($"У узла {currentNode.Id} нет дуги с предикатом {selectedPredicate}");
                        break;
                    }

                    var targetNode = graph.Nodes.FirstOrDefault(n => n.Id == selectedEdge.TargetNodeId);
                    if (targetNode == null)
                    {
                        view.ShowAnimationError($"Целевой узел {selectedEdge.TargetNodeId} не существует");
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(selectedEdge.DelaySeconds), token);

                    view.UnhighlightNode(currentNode.Id);
                    view.NotifyAnimationStepCompleted(currentNode.Id, targetNode.Id, selectedEdge.AbsoluteId);
                    currentNode = targetNode;
                }

                // ✅ помечаем нормальное завершение только если вышли по времени, а не по break
                if (_isRunning)
                    finishedNormally = true;
            }
            catch (OperationCanceledException)
            {
                // остановка по кнопке «Остановить» — не ошибка
            }
            catch (Exception ex)
            {
                view.ShowAnimationError($"Ошибка анимации: {ex.Message}");
            }
            finally
            {
                foreach (var node in graph.Nodes)
                    view.UnhighlightNode(node.Id);
                _isRunning = false;

                if (finishedNormally)
                    view.NotifyAnimationFinished(); // ✅ только при штатном завершении
            }
        }
    }
}
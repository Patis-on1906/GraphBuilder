using GraphBuilder.Interfaces;
using GraphBuilder.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private async Task RunAnimation(Graph graph, IGraphAnimationView view, double durationSeconds, CancellationToken token)
        {
            try
            {
                // 1 - поиск узла с Id = 1
                var currentNode = graph.Nodes.FirstOrDefault(n => n.Id == 1);
                if (currentNode == null)
                {
                    view.ShowAnimationError("Стартовый узел (Id=1) не найден в графе");
                    Stop();
                    return;
                }

                var startTime = DateTime.UtcNow;
                var random = new Random();

                while (_isRunning && (DateTime.UtcNow - startTime).TotalSeconds < durationSeconds)
                {
                    // 2 - подсветка узла
                    view.HighlightNode(currentNode.Id);
                    view.NotifyAnimationStepCompleted(-1, currentNode.Id, -1); // уведомление о входе

                    // 3 - пусто

                    // Шаг 4-5: выбрать случайную дугу на основе предиката
                    var outgoingEdges = currentNode.OutgoingEdges;
                    if (outgoingEdges.Count == 0)
                    {
                        view.ShowAnimationError($"Узел {currentNode.Id} не имеет исходящих дуг, анимация остановлена");
                        break;
                    }

                    int selectedPredicate = random.Next(1, outgoingEdges.Count + 1);
                    var selectedEdge = outgoingEdges.FirstOrDefault(e => e.Predicate == selectedPredicate);
                    if (selectedEdge == null)
                    {
                        view.ShowAnimationError($"У узла {currentNode.Id} нет дуги с предикатом {selectedPredicate}");
                        break;
                    }

                    // Шаг 6: определить целевой узел
                    var targetNode = graph.Nodes.FirstOrDefault(n => n.Id == selectedEdge.TargetNodeId);
                    if (targetNode == null)
                    {
                        view.ShowAnimationError($"Целевой узел {selectedEdge.TargetNodeId} не существует");
                        break;
                    }

                    // Шаг 7: задержка (с возможностью отмены)
                    await Task.Delay(TimeSpan.FromSeconds(selectedEdge.DelaySeconds), token);

                    // Шаг 8: снять подсветку с текущего узла
                    view.UnhighlightNode(currentNode.Id);
                    view.NotifyAnimationStepCompleted(currentNode.Id, targetNode.Id, selectedEdge.AbsoluteId);

                    // Шаг 9: переход к следующему узлу
                    currentNode = targetNode;
                }
            }
            catch (OperationCanceledException)
            {
                // Остановка по запросу
            }
            catch (Exception ex)
            {
                view.ShowAnimationError($"Ошибка анимации: {ex.Message}");
            }
            finally
            {
                // Снять подсветку со всех узлов
                foreach (var node in graph.Nodes)
                    view.UnhighlightNode(node.Id);
                _isRunning = false;
            }
        }
    }
}
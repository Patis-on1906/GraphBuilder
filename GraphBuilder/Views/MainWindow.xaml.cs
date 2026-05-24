using System.Windows;
using GraphBuilder.Interfaces;
using GraphBuilder.Models;
using GraphBuilder.Services;
using GraphBuilder.Editor;

namespace GraphBuilder.Views;

/// <summary>
/// Главное окно приложения. Реализует интерфейс IGraphAnimationView
/// для получения обратной связи от анимационного сервиса.
/// Содержит меню, Canvas для отрисовки графа и строку состояния.
/// </summary>
public partial class MainWindow : Window, IGraphAnimationView
{
    private readonly GraphEditorController _controller;
    private readonly Graph _graph = new();
    private readonly AnimationService _animationService = new();
    private double _animationDuration = 15.0;                   

    public MainWindow()
    {
        InitializeComponent();
        _controller = new GraphEditorController(this, GraphCanvas, _graph, _animationService);
    }

    #region IGraphAnimationView
    public void HighlightNode(int nodeId) => Dispatcher.Invoke(() =>
    {
        _controller.Renderer.HighlightNode(nodeId, true);
        StatusTextBlock.Text = $"▶ Анимация: активный узел {nodeId}";
    });

    public void UnhighlightNode(int nodeId) => Dispatcher.Invoke(() =>
    {
        _controller.Renderer.HighlightNode(nodeId, false);
    });

    public void NotifyAnimationFinished() => Dispatcher.Invoke(() =>
    {
        _controller.StopAnimation();
        StatusTextBlock.Text = "✓ Анимация завершена. Режим редактирования.";
    });

    public void NotifyAnimationStepCompleted(int fromNodeId, int toNodeId, int edgeId) => Dispatcher.Invoke(() =>
        StatusTextBlock.Text = fromNodeId < 0
            ? $"▶ Анимация: старт с узла {toNodeId}"
            : $"▶ Анимация: переход {fromNodeId} → {toNodeId} (дуга {edgeId})");

    public void ShowAnimationError(string message) => Dispatcher.Invoke(() =>
        MessageBox.Show(this, message, "Ошибка анимации", MessageBoxButton.OK, MessageBoxImage.Error));
    #endregion

    #region Menu Handlers
    private void MenuCreate_Click(object sender, RoutedEventArgs e)
    {
        _controller.CreateNewGraph();
        StatusTextBlock.Text = "Создан новый граф.";
    }

    private void MenuOpen_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "XML files (*.xml)|*.xml", 
            Title = "Открыть граф"
        };

        if (dlg.ShowDialog() == true)
        {
            bool success = GraphXmlService.LoadInto(_graph, out var error, dlg.FileName);
            if (!success)
            {
                MessageBox.Show(this, error, "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _controller.RebuildGraphService();
            _controller.Renderer.Clear();           
            _controller.Renderer.RenderAll(_graph);
            _controller.IsEditing = true;

            StatusTextBlock.Text = $"Граф загружен: {System.IO.Path.GetFileName(dlg.FileName)}";
        }
    }

    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "XML files (*.xml)|*.xml",
            DefaultExt = ".xml",
            FileName = "Graph",
            Title = "Сохранить граф"
        };

        if (dlg.ShowDialog() == true)
        {
            GraphXmlService.Save(out var error, _graph, dlg.FileName);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(this, error, "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            StatusTextBlock.Text = $"Граф сохранён: {System.IO.Path.GetFileName(dlg.FileName)}";
        }
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();
    
    private void MenuAnimationSettings_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new AnimationSettingsDialog(_animationDuration) { Owner = this };
        if (dlg.ShowDialog() == true && dlg.ResultDuration.HasValue)
        {
            _animationDuration = dlg.ResultDuration.Value;
            StatusTextBlock.Text = $"Длительность анимации: {_animationDuration} сек.";
        }
    }
    
    private void MenuStart_Click(object sender, RoutedEventArgs e)
    {
        _controller.StartAnimation(_animationDuration);
        StatusTextBlock.Text = "▶ Анимация запущена...";
    }

    private void MenuStop_Click(object sender, RoutedEventArgs e)
    {
        _controller.StopAnimation();
        StatusTextBlock.Text = "■ Анимация остановлена. Режим редактирования.";
    }
    
    private void MenuHelp_Click(object sender, RoutedEventArgs e)
    {
        var helpWindow = new UserGuideDialog { Owner = this };
        helpWindow.ShowDialog();
    }
    #endregion
}
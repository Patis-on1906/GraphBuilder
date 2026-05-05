using System.Windows;
using GraphBuilder.Interfaces;
using GraphBuilder.Models;
using GraphBuilder.Services;
using GraphBuilder.Editor;

namespace GraphBuilder.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IGraphAnimationView
{
    private readonly GraphEditorController _controller;
    private readonly Graph _graph = new();
    private readonly AnimationService _animationService;
    private readonly GraphXmlService _xmlService;
    
    public MainWindow()
    {
        InitializeComponent();
        
        _controller = new GraphEditorController(this, GraphCanvas, _graph, _animationService);
    }
    
    #region IGraphAnimationView Implementation
    public void HighlightNode(int nodeId) => Dispatcher.Invoke(() => 
        StatusTextBlock.Text= $"▶ Анимация: Подсвечен узел {nodeId}");
    
    public void UnhighlightNode(int nodeId) => Dispatcher.Invoke(() => 
        StatusTextBlock.Text = $"▶ Анимация: Узел {nodeId} сброшен");

    public void ShowAnimationError(string message) => Dispatcher.Invoke(() => 
        MessageBox.Show(this, message, "Ошибка анимации", MessageBoxButton.OK, MessageBoxImage.Error));

    public void NotifyAnimationStepCompleted(int fromNodeId, int toNodeId, int edgeId) => Dispatcher.Invoke(() => 
        StatusTextBlock.Text = $"▶ Анимация: Переход {fromNodeId} → {toNodeId} (Дуга {edgeId})");
    #endregion
    
    #region Menu Handlers
    private void MenuCreate_Click(object sender, RoutedEventArgs e)
    {
        _controller.CreateNewGraph();
        StatusTextBlock.Text = "Создан новый граф.";
    }
    
    private void MenuOpen_Click(object sender, RoutedEventArgs e) => 
        MessageBox.Show("Функция 'Открыть'", "Информация");

    private void MenuSave_Click(object sender, RoutedEventArgs e) => 
        MessageBox.Show("Функция 'Сохранить'", "Информация");

    private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();

    private void MenuAnimationSettings_Click(object sender, RoutedEventArgs e) => 
        MessageBox.Show("Диалог настроек анимации", "Информация");

    private void MenuStart_Click(object sender, RoutedEventArgs e) => _controller.StartAnimation(15.0);
    private void MenuStop_Click(object sender, RoutedEventArgs e) => _controller.StopAnimation();
    #endregion
}
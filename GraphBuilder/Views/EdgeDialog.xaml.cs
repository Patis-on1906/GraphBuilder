using System.Windows;

namespace GraphBuilder.Views;

/// <summary>
/// Диалоговое окно для изменения предиката и задержки дуги.
/// </summary>
public partial class EdgeDialog : Window
{
    public int? ResultPredicate { get; private set; }
    public double? ResultDelay { get; private set; }
    private readonly int _maxPredicate;
    
    public EdgeDialog(int? currentPredicate = null, double? currentDelay = null, int maxPredicate = 1)
    {
        InitializeComponent();
        _maxPredicate = maxPredicate;
        PredicateTextBox.Text = currentPredicate?.ToString() ?? "1";
        DelayTextBox.Text = currentDelay?.ToString() ?? "2.0";
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PredicateTextBox.Text, out int predicate) || predicate < 1 || predicate > _maxPredicate)
        {
            MessageBox.Show($"Предикат должен быть целым числом от 1 до {_maxPredicate}", "Ошибка");
            return;
        }
        if (!double.TryParse(DelayTextBox.Text, out double delay) || delay <= 0)
        {
            MessageBox.Show("Задержка должна быть числом > 0", "Ошибка");
            return;
        }
        
        ResultPredicate = predicate;
        ResultDelay = delay;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
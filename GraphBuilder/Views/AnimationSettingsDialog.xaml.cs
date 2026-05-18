using System.Windows;

namespace GraphBuilder.Views;

public partial class AnimationSettingsDialog : Window
{
    public double? ResultDuration { get; private set; }

    public AnimationSettingsDialog(double? currentDuration = null)
    {
        InitializeComponent();
        DurationTextBox.Text = currentDuration?.ToString() ?? "15";
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(DurationTextBox.Text, out double duration) || duration <= 0)
        {
            MessageBox.Show("Длительность должна быть числом > 0", "Ошибка");
            return;
        }
        ResultDuration = duration;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
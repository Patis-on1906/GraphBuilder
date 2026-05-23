using System.Windows;

namespace GraphBuilder.Views;

public partial class NodeCodeDialog : Window
{
    public string? ResultCode { get; private set; }

    public NodeCodeDialog(string? currentCode = null)
    {
        InitializeComponent();
        CodeTextBox.Text = currentCode ?? "random(1,N)";
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        ResultCode = CodeTextBox.Text?.Trim();
        if (string.IsNullOrEmpty(ResultCode))
        {
            MessageBox.Show("Код не может быть пустым", "Ошибка", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
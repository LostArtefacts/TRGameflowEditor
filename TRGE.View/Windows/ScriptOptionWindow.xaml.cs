using System.Windows;
using TRGE.Core;
using TRGE.View.Utils;

namespace TRGE.View.Windows;

/// <summary>
/// Interaction logic for ScriptOptionWindow.xaml
/// </summary>
public partial class ScriptOptionWindow : Window
{
    public TRScriptOpenOption Option { get; private set; }

    public ScriptOptionWindow()
    {
        InitializeComponent();
        Owner = WindowUtils.GetActiveWindow(this);
        Option = TRScriptOpenOption.DiscardBackup;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowUtils.TidyMenu(this);
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        Option = (_discardOption.IsChecked ?? false) ? TRScriptOpenOption.DiscardBackup : TRScriptOpenOption.RestoreBackup;
        DialogResult = true;
    }
}
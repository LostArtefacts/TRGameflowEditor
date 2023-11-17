using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace TRGE.View.Controls;

/// <summary>
/// Interaction logic for DefaultManualControl.xaml
/// </summary>
public partial class DefaultManualControl : UserControl
{
    #region Dependency Properties
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register
    (
        "Text", typeof(string), typeof(DefaultManualControl)
    );

    public static readonly DependencyProperty DefaultLabelProperty = DependencyProperty.Register
    (
        "DefaultLabel", typeof(string), typeof(DefaultManualControl), new PropertyMetadata("Use the default configuration")
    );

    public static readonly DependencyProperty ManualLabelProperty = DependencyProperty.Register
    (
        "ManualLabel", typeof(string), typeof(DefaultManualControl), new PropertyMetadata("Configure manually")
    );

    public static readonly DependencyProperty IsDefaultProperty = DependencyProperty.Register
    (
        "IsDefault", typeof(bool), typeof(DefaultManualControl), new PropertyMetadata(true)
    );

    public static readonly DependencyProperty IsManualProperty = DependencyProperty.Register
    (
        "IsManual", typeof(bool), typeof(DefaultManualControl), new PropertyMetadata(false)
    );

    public static readonly DependencyProperty ManualButtonTextProperty = DependencyProperty.Register
    (
        "ManualButtonText", typeof(string), typeof(DefaultManualControl), new PropertyMetadata("Organise")
    );

    public static readonly DependencyProperty IsViableProperty = DependencyProperty.Register
    (
        "IsViable", typeof(bool), typeof(DefaultManualControl), new PropertyMetadata(true)
    );

    public static readonly DependencyProperty WarningIconProperty = DependencyProperty.Register
    (
        "WarningIcon", typeof(BitmapSource), typeof(DefaultManualControl)
    );

    public static readonly DependencyProperty UnviableTextProperty = DependencyProperty.Register
    (
        "UnviableText", typeof(string), typeof(DefaultManualControl), new PropertyMetadata("This setting is controlled by an external editor. Clicking Edit will move control to this editor, but any external changes will be lost.")
    ); 

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string DefaultLabel
    {
        get => (string)GetValue(DefaultLabelProperty);
        set => SetValue(DefaultLabelProperty, value);
    }

    public string ManualLabel
    {
        get => (string)GetValue(ManualLabelProperty);
        set => SetValue(ManualLabelProperty, value);
    }

    public bool IsDefault
    {
        get => (bool)GetValue(IsDefaultProperty);
        set => SetValue(IsDefaultProperty, value);
    }

    public bool IsManual
    {
        get => (bool)GetValue(IsManualProperty);
        set => SetValue(IsManualProperty, value);
    }

    public bool IsViable
    {
        get => (bool)GetValue(IsViableProperty);
        set => SetValue(IsViableProperty, value);
    }

    public BitmapSource WarningIcon
    {
        get => (BitmapSource)GetValue(WarningIconProperty);
        set => SetValue(WarningIconProperty, value);
    }

    public string UnviableText
    {
        get => (string)GetValue(UnviableTextProperty);
        set => SetValue(UnviableTextProperty, value);
    }
    #endregion

    #region Events
    public static readonly RoutedEvent ManualConfigureEvent = EventManager.RegisterRoutedEvent
    (
        "ManualConfigure", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DefaultManualControl)
    );

    public static readonly RoutedEvent ChangeViabilityEvent = EventManager.RegisterRoutedEvent
    (
        "ChangeViability", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DefaultManualControl)
    );

    public event RoutedEventHandler ManualConfigure
    {
        add => AddHandler(ManualConfigureEvent, value);
        remove => RemoveHandler(ManualConfigureEvent, value);
    }

    public event RoutedEventHandler ChangeViability
    {
        add => AddHandler(ChangeViabilityEvent, value);
        remove => RemoveHandler(ChangeViabilityEvent, value);
    }
    #endregion

    public DefaultManualControl()
    {
        InitializeComponent();
        _content.DataContext = this;
        WarningIcon = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Warning.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }

    private void ManualButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ManualConfigureEvent));
    }

    private void ViabilityButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ChangeViabilityEvent));
    }
}
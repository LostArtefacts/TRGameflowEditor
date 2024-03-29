﻿using System.Windows;
using TRGE.View.Model.Data;
using TRGE.View.Utils;

namespace TRGE.View.Windows;

/// <summary>
/// Interaction logic for UnarmedLevelsWindow.xaml
/// </summary>
public partial class UnarmedLevelsWindow : Window
{
    #region Dependency Properties
    public static readonly DependencyProperty SequencingProperty = DependencyProperty.Register
    (
        "LevelData", typeof(FlaggedLevelData), typeof(UnarmedLevelsWindow)
    );

    public FlaggedLevelData LevelData
    {
        get => (FlaggedLevelData)GetValue(SequencingProperty);
        set => SetValue(SequencingProperty, value);
    }
    #endregion

    public UnarmedLevelsWindow(FlaggedLevelData levelData)
    {
        InitializeComponent();
        Owner = WindowUtils.GetActiveWindow(this);
        DataContext = this;

        LevelData = levelData;

        MinHeight = Height;
        MinWidth = Width;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowUtils.EnableMinimiseButton(this, false);
        WindowUtils.TidyMenu(this);
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    //this works, in that it means the whole checkbox label triggers the checkbox, but it
    //is very hacky and the list selection seems to get screw up
    /*private void ListView_MouseUp(object sender, MouseButtonEventArgs e)
    {
        ListViewItem item = ControlUtils.GetItemAt(_listView, e.GetPosition(_listView));
        if (item != null)
        {
            MutableTuple<string, string, bool> data = (MutableTuple<string, string, bool>)item.DataContext;
            data.Item3 = !data.Item3;
            LevelData = new List<MutableTuple<string, string, bool>>(LevelData);
            _listView.UnselectAll();
        }
    }*/
}
using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;

namespace TRGE.View.Utils
{
    public static class WindowUtils
    {
        public static IntPtr GetWindowHandle(Window window)
        {
            return new WindowInteropHelper(window).Handle;
        }

        public static IntPtr GetActiveWindowHandle()
        {
            Window w = GetActiveWindow();
            return w == null ? IntPtr.Zero : GetWindowHandle(w);
        }

        public static Window GetActiveWindow()
        {
            return Application.Current.Windows.OfType<Window>().SingleOrDefault(e => e.IsActive);
        }

        public static void ShowWarning(string message)
        {
            ShowWarning(GetActiveWindow(), message);
        }

        public static void ShowError(string message)
        {
            ShowError(GetActiveWindow(), message);
        }

        public static bool ShowConfirm(string message)
        {
            return ShowConfirm(GetActiveWindow(), message);
        }

        public static void ShowWarning(Window window, string message)
        {
            MessageBox.Show(window, message, "TRGE", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(Window window, string message)
        {
            MessageBox.Show(window, message, "TRGE", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool ShowConfirm(Window window, string message)
        {
            return MessageBox.Show(window, message, "TRGE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
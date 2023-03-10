using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using TRGE.Coord;
using TRGE.Core;
using TRGE.View.Utils;
using TRGE.View.Windows;

namespace TRGE.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private DownloadingWindow _downloadingWindow;

        public string Version { get; private set; }
        public string TaggedVersion { get; private set; }

        public App()
        {
            WindowUtils.SetMenuAlignment();
            TRCoord.Instance.ResourceDownloading += TRCoord_ResourceDownloading;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Version v = assembly.GetName().Version;
            Version = string.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);
            TaggedVersion = "V" + Version;

            TRInterop.ExecutingVersion = Version;
            TRInterop.TaggedVersion = TaggedVersion;
            TRInterop.RandomisationSupported = false;
        }

        private void TRCoord_ResourceDownloading(object sender, TRDownloadEventArgs e)
        {
            if (e.Status == TRDownloadStatus.Initialising)
            {
                new Thread(LaunchDownloadDialog).Start();
            }
            else if (_downloadingWindow != null)
            {
                Dispatcher.Invoke(delegate
                {
                    _downloadingWindow.HandleDownloadEvent(e);
                });
            }
        }

        private void LaunchDownloadDialog()
        {
            Dispatcher.Invoke(delegate
            {
                (_downloadingWindow = new DownloadingWindow()).ShowDialog();
                _downloadingWindow = null;
            });
        }
    }
}
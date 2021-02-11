using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public App()
        {
            WindowUtils.SetMenuAlignment();
            TRCoord.Instance.ResourceDownloading += TRCoord_ResourceDownloading;
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
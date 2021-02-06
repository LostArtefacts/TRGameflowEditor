using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TRGE.Coord;
using TRGE.View.Model;
using TRGE.View.Utils;

namespace TRGE.View.Controls
{
    /// <summary>
    /// Interaction logic for EditorControl.xaml
    /// </summary>
    public partial class EditorControl : UserControl
    {
        public TREditor Editor { get; private set; }

        public EditorControl()
        {
            InitializeComponent();
        }

        public void Load(DataFolderEventArgs e)
        {
            Editor = e.Editor;
        }

        public void Unload()
        {
            Editor = null;
        }

        public void OpenBackupFolder()
        {
            Process.Start("explorer.exe", Editor.BackupDirectory);
        }

        public void RestoreDefaults()
        {
            if (WindowUtils.ShowConfirm("The files that were backed up when this folder was first opened will be copied back to the original directory.\n\nDo you wish to proceed?"))
            {
                Editor.Restore();
            }
        }

        public void ExportSettings()
        {

        }

        public void ImportSettings()
        {

        }
    }
}
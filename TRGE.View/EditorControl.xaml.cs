using System;
using System.Collections.Generic;
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

namespace TRGE.View
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
    }
}
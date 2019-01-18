using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bestiary.ViewWindows
{
    /// <summary>
    /// Interaction logic for GatherTypeControl.xaml
    /// </summary>
    public partial class GatherTypeControl : UserControl
    {
        public GatherTypeControl()
        {
            InitializeComponent();
        }

        public void ChangeSelectedRow(object Sender, MouseButtonEventArgs e)
        {
            var dgrow = (DataGridRow)Sender;
            DependencyObject parent = VisualTreeHelper.GetParent((Visual)e.OriginalSource);
            while (parent as DataGrid == null && parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent as DataGrid != null)
            {
                var dataGrid = (DataGrid)parent;
                dataGrid.SelectedIndex = dgrow.GetIndex();
            }
        }
    }
}
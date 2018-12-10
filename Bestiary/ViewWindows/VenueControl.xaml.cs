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

namespace Bestiary.ViewWindows
{
    /// <summary>
    /// Interaction logic for VenueControl.xaml
    /// </summary>
    public partial class VenueControl : UserControl
    {
        public VenueControl()
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

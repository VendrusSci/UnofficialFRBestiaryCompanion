using Bestiary.Model;
using Bestiary.ViewModel.Dataviews;
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
using System.Windows.Shapes;

namespace Bestiary.ViewWindows
{
    /// <summary>
    /// Interaction logic for FRBestiaryView.xaml
    /// </summary>
    public partial class FRBestiaryView : Window
    {
        public FRBestiaryView(IModel model, OwnershipStatus[] availableOwnedStatus, BondingLevels[] availableBondingLevels, LocationTypes[] availableLocationTypes)
        {
            InitializeComponent();
            DataContext = new FRBestiaryViewModel(model, availableOwnedStatus, availableBondingLevels, availableLocationTypes);
        }
    }
}

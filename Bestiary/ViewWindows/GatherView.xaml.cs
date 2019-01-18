using Bestiary.Model;
using Bestiary.ViewModel;
using System.Windows;

namespace Bestiary.ViewWindows
{
    /// <summary>
    /// Interaction logic for GatherView.xaml
    /// </summary>
    public partial class GatherView : Window
    {
        public GatherView(IModel model, OwnershipStatus[] availableOwnedStatus, BondingLevels[] availableBondingLevels, LocationTypes[] availableLocationTypes)
        {
            InitializeComponent();
            DataContext = new GatherViewModel(model, availableOwnedStatus, availableBondingLevels, availableLocationTypes);
        }
    }
}
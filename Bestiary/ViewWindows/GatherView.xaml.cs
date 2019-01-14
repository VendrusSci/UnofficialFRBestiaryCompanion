using Bestiary.Model;
using Bestiary.Services;
using Bestiary.ViewModel.Dataviews;
using System.Windows;

namespace Bestiary.ViewWindows
{
    /// <summary>
    /// Interaction logic for GatherView.xaml
    /// </summary>
    public partial class GatherView : Window
    {
        public GatherView(IModel model, OwnershipStatus[] availableOwnedStatus, BondingLevels[] availableBondingLevels, LocationTypes[] availableLocationTypes, Theme theme)
        {
            InitializeComponent();
            DataContext = new GatherViewModel(model, availableOwnedStatus, availableBondingLevels, availableLocationTypes, theme);
        }
    }
}

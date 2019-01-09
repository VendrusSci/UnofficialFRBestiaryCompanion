using Bestiary.Model;
using Bestiary.Services;
using Bestiary.ViewModel.Dataviews;
using System.Windows;

namespace Bestiary.ViewWindows
{
    /// <summary>
    /// Interaction logic for ColiseumView.xaml
    /// </summary>
    public partial class ColiseumView : Window
    {
        public ColiseumView(IModel model, OwnershipStatus[] availableOwnedStatus, BondingLevels[] availableBondingLevels, LocationTypes[] availableLocationTypes, Theme theme)
        {
            InitializeComponent();
            DataContext = new ColiseumViewModel(model, availableOwnedStatus, availableBondingLevels, availableLocationTypes, theme);
        }
    }
}

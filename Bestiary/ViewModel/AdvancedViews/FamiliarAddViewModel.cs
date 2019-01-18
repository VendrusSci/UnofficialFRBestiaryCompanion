using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Bestiary.ViewModel
{
    class FamiliarAddViewModel : INotifyPropertyChanged
    {
        public FamiliarAddParameters FamiliarParameters { set; get; }

        public string Name { get; set; }
        public int? Id { get; set; }
        public string FlavourText { get; set; }
        public Brush InfoTextColour { get; private set; }
        public string StatusInfo { get; private set; }

        private IModel m_Model;

        public FamiliarAddViewModel(IModel model)
        {
            FamiliarParameters = new FamiliarAddParameters();
            m_Model = model;
            InfoTextColour = Brushes.Black;
            StatusInfo = "";
            Name = null;
            Id = null;
            FlavourText = null;
        }

        private LambdaCommand m_AddFamiliar;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddFamiliar
        {
            get
            {
                if(m_AddFamiliar == null)
                {
                    m_AddFamiliar = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            try
                            {
                                MainViewModel.UserActionLog.Info("Adding familiar started:");
                                MainViewModel.UserActionLog.Info($" Source: {FamiliarParameters.SelectedSource}");
                                IFamiliarSource source = new JoxarSpareInventory();
                                switch (FamiliarParameters.SelectedSource)
                                {
                                    case Sources.Baldwin:
                                        MainViewModel.UserActionLog.Info($"     Info: {FamiliarParameters.SelectedLevel.Value}");
                                        source = new Baldwin(FamiliarParameters.SelectedLevel.Value);
                                        break;
                                    case Sources.Coliseum:
                                        MainViewModel.UserActionLog.Info($"     Info: {FamiliarParameters.SelectedVenueName}, {FamiliarParameters.SelectedEnemyType.Value}");
                                        source = new Coliseum(FamiliarParameters.SelectedVenueName, FamiliarParameters.SelectedEnemyType.Value);
                                        break;
                                    case Sources.Event:
                                        MainViewModel.UserActionLog.Info($"     Info: {FamiliarParameters.SelectedSiteEvent}, {FamiliarParameters.SelectedCycleYear}");
                                        source = new SiteEvent(FamiliarParameters.SelectedSiteEvent, FamiliarParameters.SelectedCycleYear);
                                        break;
                                    case Sources.Festival:
                                        MainViewModel.UserActionLog.Info($"     Info: {FamiliarParameters.SelectedFlight.Value}, {FamiliarParameters.SelectedCycleYear}");
                                        source = new Festival(FamiliarParameters.SelectedFlight.Value, FamiliarParameters.SelectedCycleYear);
                                        break;
                                    case Sources.Gathering:
                                        MainViewModel.UserActionLog.Info($"     Info: {FamiliarParameters.GatherControl.GetSelectedFlights()}, {FamiliarParameters.SelectedGatherType.Value}, {FamiliarParameters.SelectedLevel.Value}");
                                        source = new Gathering(FamiliarParameters.GatherControl.GetSelectedFlights(), FamiliarParameters.SelectedGatherType.Value, FamiliarParameters.SelectedLevel.Value);
                                        break;
                                    case Sources.Joxar:
                                        source = new JoxarSpareInventory();
                                        break;
                                    case Sources.Marketplace:
                                        MainViewModel.UserActionLog.Info($"     Info: {FamiliarParameters.SelectedCurrency.Value}");
                                        source = new Marketplace(FamiliarParameters.SelectedCurrency.Value);
                                        break;
                                    case Sources.Swipp:
                                        source = new Swipp();
                                        break;
                                }
                                MainViewModel.UserActionLog.Info($" Familiar: {Name}, {FamiliarParameters.SelectedAvailability.Value}, {Id.Value}");
                                Familiar familiar = new Familiar(Name, source, FamiliarParameters.SelectedAvailability.Value, FlavourText, Id.Value);
                                if (Id != null)
                                {
                                    if (m_Model.LookupFamiliar(Id.Value) != null)
                                    {
                                        InfoTextColour = Brushes.DarkRed;
                                        StatusInfo = "Error: Familiar already exists";
                                    }
                                    else
                                    {
                                        m_Model.AddFamiliar(familiar);
                                        InfoTextColour = Brushes.Black;
                                    }
                                }
                                else
                                {
                                    InfoTextColour = Brushes.DarkRed;
                                    StatusInfo = "Error: Invalid data";
                                }
                            }
                            catch
                            {
                                InfoTextColour = Brushes.DarkRed;
                                StatusInfo = "Error: Invalid data";
                            }
                            Name = "";
                            FlavourText = "";
                            Id = null;
                        }
                    );
                }
                return m_AddFamiliar;
            }
        }
    }
}

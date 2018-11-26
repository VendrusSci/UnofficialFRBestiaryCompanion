using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bestiary.ViewModel
{
    class FamiliarAddViewModel : INotifyPropertyChanged
    {
        public FamiliarAddParameters FamiliarParameters { set; get; }

        public string Name { get; set; }
        public int? Id { get; set; }
        public string FlavourText { get; set; }

        private IModel m_Model;

        public FamiliarAddViewModel(IModel model)
        {
            FamiliarParameters = new FamiliarAddParameters();
            m_Model = model;
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
                            MainViewModel.UserActionLog.Info("Adding familiar started:");
                            MainViewModel.UserActionLog.Info($" Source: {FamiliarParameters.SelectedSource}");
                            IFamiliarSource source = new JoxarSpareInventory();
                            switch(FamiliarParameters.SelectedSource)
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
                            m_Model.AddFamiliar(familiar);
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

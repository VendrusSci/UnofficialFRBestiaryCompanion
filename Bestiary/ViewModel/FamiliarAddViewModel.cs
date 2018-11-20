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
                            IFamiliarSource source = new JoxarSpareInventory();
                            switch(FamiliarParameters.SelectedSource)
                            {
                                case Sources.Baldwin:
                                    source = new Baldwin(FamiliarParameters.SelectedLevel.Value);
                                    break;
                                case Sources.Coliseum:
                                    source = new Coliseum(FamiliarParameters.SelectedVenueName, FamiliarParameters.SelectedEnemyType.Value);
                                    break;
                                case Sources.Event:
                                    source = new SiteEvent(FamiliarParameters.SelectedSiteEvent, FamiliarParameters.SelectedCycleYear);
                                    break;
                                case Sources.Festival:
                                    source = new Festival(FamiliarParameters.SelectedFlight.Value, FamiliarParameters.SelectedCycleYear);
                                    break;
                                case Sources.Gathering:
                                    source = new Gathering(FamiliarParameters.GatherControl.GetSelectedFlights(), FamiliarParameters.SelectedGatherType.Value, FamiliarParameters.SelectedLevel.Value);
                                    break;
                                case Sources.Joxar:
                                    source = new JoxarSpareInventory();
                                    break;
                                case Sources.Marketplace:
                                    source = new Marketplace(FamiliarParameters.SelectedCurrency.Value);
                                    break;
                                case Sources.Swipp:
                                    source = new Swipp();
                                    break;
                            }
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

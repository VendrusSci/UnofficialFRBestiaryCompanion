using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bestiary.ViewModel
{
    class FamiliarDataViewModel : INotifyPropertyChanged
    {
        public FamiliarViewModel FamiliarDisplayInfo { get; set; }
        public Familiar Familiar { get; set; }
        public BondingLevels? SelectedBondingLevel { get; set; }
        public LocationTypes? SelectedLocation { get; set; }
        public OwnershipStatus? SelectedOwnershipStatus { get; set; }

        private IModel m_Model;
        private ICRUD<OwnedFamiliar> OwnedFamiliar;

        public FamiliarDataViewModel(FamiliarViewModel info, IModel model)
        {
            MainViewModel.UserActionLog.Info($"Familiar data view started");
            m_Model = model;
            FamiliarDisplayInfo = info;
            Familiar = FamiliarDisplayInfo.Info.Familiar;
            OwnedFamiliar = FamiliarDisplayInfo.Info.OwnedFamiliar;
            SelectedBondingLevel = FamiliarDisplayInfo.Info.BondLevel;
            SelectedLocation = FamiliarDisplayInfo.Info.Location;
            SelectedOwnershipStatus = FamiliarDisplayInfo.Info.Owned;
        }

        private LambdaCommand m_UpdateStatus;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand UpdateStatus
        {
            get
            {
                if(m_UpdateStatus == null)
                {
                    m_UpdateStatus = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            MainViewModel.UserActionLog.Info($"Updating status:");
                            if (FamiliarDisplayInfo.Info.Owned == OwnershipStatus.Owned)
                            {
                                if (SelectedOwnershipStatus == OwnershipStatus.NotOwned)
                                {
                                    MainViewModel.UserActionLog.Info($"Familiar no longer owned");
                                    FamiliarDisplayInfo.Info.OwnedFamiliar.Delete();
                                }
                                else
                                {
                                    MainViewModel.UserActionLog.Info($"Familiar status updated: {SelectedLocation.Value}, {SelectedBondingLevel.Value}");
                                    OwnedFamiliar.Update(f => f.Location = SelectedLocation.Value);
                                    OwnedFamiliar.Update(f => f.BondingLevel = SelectedBondingLevel.Value);
                                }
                            }
                            else
                            {
                                if (SelectedOwnershipStatus == OwnershipStatus.Owned)
                                {
                                    MainViewModel.UserActionLog.Info($"Familiar now owned");
                                    m_Model.AddOwnedFamiliar(new OwnedFamiliar(Familiar.Id, SelectedBondingLevel ?? BondingLevels.Wary, SelectedLocation ?? LocationTypes.InHoard));
                                    FamiliarDisplayInfo.Info.OwnedFamiliar = m_Model.LookupOwnedFamiliar(Familiar.Id);
                                    OwnedFamiliar = FamiliarDisplayInfo.Info.OwnedFamiliar;
                                    SelectedBondingLevel = FamiliarDisplayInfo.Info.BondLevel;
                                    SelectedLocation = FamiliarDisplayInfo.Info.Location;
                                }
                            }
                        }
                    );
                }
                return m_UpdateStatus;
            }
        }
    }
}

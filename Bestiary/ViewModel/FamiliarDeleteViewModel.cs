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
    class FamiliarDeleteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public int Id { get; set; }
        private ICRUD<Familiar> m_KnownFamiliar;
        private ICRUD<OwnedFamiliar> m_OwnedFamiliar;
        private IModel m_Model;

        public FamiliarDeleteViewModel(IModel model)
        {
            MainViewModel.UserActionLog.Info($"Familiar delete window started");
            m_Model = model;
        }

        private LambdaCommand m_LoadName;
        public ICommand LoadName
        {
            get
            {
                if(m_LoadName == null)
                {
                    m_LoadName = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            MainViewModel.UserActionLog.Info($"Familiar Id looked up: {Id}");
                            m_KnownFamiliar = m_Model.LookupFamiliar(Id);
                            if(m_KnownFamiliar != null)
                            {
                                Name = m_KnownFamiliar.Fetch().Name;
                                MainViewModel.UserActionLog.Info($"Familiar found: {Name}");
                            }
                            else
                            {
                                MainViewModel.UserActionLog.Info($"Familiar not found");
                                Name = "ID unknown";
                            }
                        }
                    );
                }
                return m_LoadName;
            }
        }

        private LambdaCommand m_DeleteFamiliar;
        public ICommand DeleteFamiliar
        {
            get
            {
                if (m_DeleteFamiliar == null)
                {
                    m_DeleteFamiliar = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            MainViewModel.UserActionLog.Info($"Deleting familiar: {Id}");
                            m_KnownFamiliar.Delete();
                            m_OwnedFamiliar = m_Model.LookupOwnedFamiliar(Id);
                            if(m_OwnedFamiliar != null)
                            {
                                MainViewModel.UserActionLog.Info($"Owned familiar found, deleting that too");
                                m_OwnedFamiliar.Delete();
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("m_OwnedFamiliar"));
                            }
                        }
                    );
                }
                return m_DeleteFamiliar;
            }
        }
    }
}

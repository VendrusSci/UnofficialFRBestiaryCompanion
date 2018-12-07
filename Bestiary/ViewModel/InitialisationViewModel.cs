using Bestiary.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Bestiary.ViewModel
{
    public class InitialisationViewModel
    {
        private IModel m_model;
        public InitialisationViewModel(IModel model) { m_model = model; }

        private LambdaCommand m_MakeAllOwned;
        public ICommand MakeAllOwned
        {
            get
            {
                if (m_MakeAllOwned == null)
                {
                    m_MakeAllOwned = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            foreach(var famId in m_model.Familiars)
                            {
                                if (m_model.LookupOwnedFamiliar(famId) == null)
                                {
                                    m_model.AddOwnedFamiliar(new OwnedFamiliar(famId, BondingLevels.Wary, LocationTypes.InHoard));
                                }   
                            }
                        }
                    );
                }
                return m_MakeAllOwned;
            }
        }

        private LambdaCommand m_MakeAllUnOwned;
        public ICommand MakeAllUnOwned
        {
            get
            {
                if (m_MakeAllUnOwned == null)
                {
                    m_MakeAllUnOwned = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            List<int> ownedCopy = m_model.OwnedFamiliars.ToList();
                            foreach(int id in ownedCopy)
                            {
                                m_model.LookupOwnedFamiliar(id).Delete();
                            }
                        }
                    );
                }
                return m_MakeAllUnOwned;
            }
        }

        private LambdaCommand m_MakeAllOwnedAwakened;
        public ICommand MakeAllOwnedAwakened
        {
            get
            {
                if (m_MakeAllOwnedAwakened == null)
                {
                    m_MakeAllOwnedAwakened = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            foreach (var famId in m_model.OwnedFamiliars)
                            {
                                var ownedFam = m_model.LookupOwnedFamiliar(famId);
                                ownedFam.Update(f => f.BondingLevel = BondingLevels.Awakened);
                            }
                        }
                    );
                }
                return m_MakeAllOwnedAwakened;
            }
        }

        private LambdaCommand m_MakeAllOwnedWary;
        public ICommand MakeAllOwnedWary
        {
            get
            {
                if (m_MakeAllOwnedWary == null)
                {
                    m_MakeAllOwnedWary = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            foreach (var famId in m_model.OwnedFamiliars)
                            {
                                var ownedFam = m_model.LookupOwnedFamiliar(famId);
                                ownedFam.Update(f => f.BondingLevel = BondingLevels.Wary);
                            }
                        }
                    );
                }
                return m_MakeAllOwnedWary;
            }
        }


    }
}

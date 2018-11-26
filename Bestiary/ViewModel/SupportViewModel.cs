using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bestiary.ViewModel
{
    class SupportViewModel
    {
        private LambdaCommand m_NavigateToUri;
        public ICommand NavigateToUri
        {
            get
            {
                if(m_NavigateToUri == null)
                {
                    m_NavigateToUri = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            MainViewModel.UserActionLog.Info($"Navigated to uri: {(string)p}");
                            Process.Start(new ProcessStartInfo((string)p));
                        }
                    );
                }
                return m_NavigateToUri;
            }
        }  
    }


}

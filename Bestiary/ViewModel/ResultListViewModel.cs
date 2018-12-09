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
    class ResultListViewModel : INotifyPropertyChanged
    {
        public string NameList { get; private set; } = "";
        public string IconList { get; private set; } = "";
        public string NameCopyButtonState => CopyButtonLabelFromCurrentText(NameList);
        public string IconCopyButtonState => CopyButtonLabelFromCurrentText(IconList);

        private string NotCopied = "Copy to Clipboard";
        private string Copied = "Text Copied!";

        private FamiliarViewModel[] m_Familiars;
        public ResultListViewModel(FamiliarViewModel[] familiars)
        {
            m_Familiars = familiars;

            foreach(var familiar in m_Familiars)
            {
                NameList += familiar.Info.Familiar.Name + ", ";
                IconList += "[item=" + familiar.Info.Familiar.Name + "]";
            }
        }

        private string CopyButtonLabelFromCurrentText(string currentText)
        {
            if(currentText == Clipboard.GetText())
            {
                return Copied;
            }
            else
            {
                return NotCopied;
            }
        }

        private LambdaCommand m_CopyToClipboard;
        public ICommand CopyToClipboard
        {
            get
            {
                if(m_CopyToClipboard == null)
                {
                    m_CopyToClipboard = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            Clipboard.SetText((string)p);
                        },
                        onCanExecute: (p) =>
                        {
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NameCopyButtonState)));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconCopyButtonState)));
                            return (string)p != Clipboard.GetText();
                        }
                    );
                }
                return m_CopyToClipboard;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

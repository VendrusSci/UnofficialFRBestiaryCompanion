using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bestiary.ViewModel
{
    class OpenDialogCommand<WindowType> : BaseCommand where WindowType: Window
    {
        public OpenDialogCommand(Window parentWindow, Func<object, WindowType> makeWindow, Predicate<object> canExecute=null, Action<WindowType> afterClosed=null)
        {
            m_ParentWindow = parentWindow;
            m_MakeWindow = makeWindow;
            m_CanExecute = canExecute;
            m_AfterClosed = afterClosed;
        }

        public override bool CanExecute(object parameter)
        {
            return m_CanExecute?.Invoke(parameter) ?? true;
        }

        public override void Execute(object parameter)
        {
            MainViewModel.UserActionLog.Info($"'{typeof(WindowType).Name}' opened");
            var newDialog = m_MakeWindow(parameter);
            newDialog.Owner = m_ParentWindow;
            newDialog.ShowDialog();
            MainViewModel.UserActionLog.Info($"'{typeof(WindowType).Name}' closed");
            m_AfterClosed?.Invoke(newDialog);
        }

        Func<object, WindowType> m_MakeWindow;
        Predicate<object> m_CanExecute;
        Action<WindowType> m_AfterClosed;
        Window m_ParentWindow;
    }
}

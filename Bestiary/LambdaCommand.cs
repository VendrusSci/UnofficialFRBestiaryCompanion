using System;
using System.Windows.Input;


namespace Bestiary
{
    class LambdaCommand : ICommand
    {
        public LambdaCommand(Action<object> onExecute, Predicate<object> onCanExecute=null)
        {
            m_onExecute = onExecute;
            m_onCanExecute = onCanExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return m_onCanExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            m_onExecute?.Invoke(parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private Action<object> m_onExecute;
        private Predicate<object> m_onCanExecute;
    }
}

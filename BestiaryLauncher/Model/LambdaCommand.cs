using System;
using System.Windows.Input;


namespace BestiaryLauncher.Model
{
    public class LambdaCommand : BaseCommand
    {
        public LambdaCommand(Action<object> onExecute, Predicate<object> onCanExecute = null)
        {
            m_onExecute = onExecute;
            m_onCanExecute = onCanExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return m_onCanExecute?.Invoke(parameter) ?? true;
        }

        public override void Execute(object parameter)
        {
            m_onExecute?.Invoke(parameter);
        }

        private Action<object> m_onExecute;
        private Predicate<object> m_onCanExecute;
    }
}

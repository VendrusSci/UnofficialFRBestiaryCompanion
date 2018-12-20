using System.ComponentModel;


namespace Bestiary.ViewModel
{
    class AboutViewModel : INotifyPropertyChanged
    {
        public string Version { get; private set; }
        public AboutViewModel(string version)
        {
            Version = version;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

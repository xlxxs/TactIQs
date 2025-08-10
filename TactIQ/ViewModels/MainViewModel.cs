using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TactIQ.ViewModels
{
    /// <summary>
    /// ViewModel für die Hauptansicht der Anwendung.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // ViewModel für die verschiedenen Ansichten
        private object? _currentViewModel;
        public object? CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

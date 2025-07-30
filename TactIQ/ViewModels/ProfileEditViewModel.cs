using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Abstractions;

namespace TactIQ.ViewModels
{
    public class ProfileEditViewModel : INotifyPropertyChanged
    {
        private readonly IOpponentRepository _repo;

        public int Id { get; }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private bool _marked;
        public bool Marked { get => _marked; set { _marked = value; OnPropertyChanged(); } }

        private string _club;
        public string Club { get => _club; set { _club = value; OnPropertyChanged(); } }


        public ICommand SaveCommand { get; }

        public ProfileEditViewModel(IOpponentRepository repo, Opponent opponent)
        {
            _repo = repo;
            Id = opponent.Id;
            _name = opponent.Name;
            _marked = opponent.Marked;
            _club = opponent.Club;

            SaveCommand = new RelayCommand(_ => Save());
        }

        private void Save()
        {
            _repo.Update(new Opponent { Id = Id, Name = Name, Marked = Marked, Club = "" });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

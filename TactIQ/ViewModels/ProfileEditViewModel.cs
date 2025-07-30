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
        private readonly INavigationService _nav;

        public int Id { get; }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private bool _marked;
        public bool Marked { get => _marked; set { _marked = value; OnPropertyChanged(); } }

        private string _club;
        public string Club { get => _club; set { _club = value; OnPropertyChanged(); } }


        public ICommand SaveCommand { get; }

        public ProfileEditViewModel(INavigationService nav,IOpponentRepository repo, Opponent opponent)
        {
            _repo = repo;
            _nav = nav;
            Id = opponent.Id;
            _name = opponent.Name;
            _marked = opponent.Marked;
            _club = opponent.Club;

            SaveCommand = new RelayCommand(_ => Save());
        }

        private void Save()
        {
            if (Id == 0 || _repo.GetById(Id) == null)
            {
                _repo.Add(Name, Club);
            }
            else
            {
                _repo.Update(new Opponent { Id = Id, Club = Club, Marked = Marked, Name = Name});
            }

            _nav.NavigateTo(new OpponentProfilesViewModel(_nav, _repo));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Miscellaneous
{
    public class Interfaces
    {
        public interface INavigationService
        {
            void NavigateTo(object viewModel);
        }

        public interface IDialogCloser
        {
            void Close(bool? dialogResult = true);
        }

        public interface IOpponentRepository
        {
            IEnumerable<Model.Opponent> GetAll();
            Model.Opponent? GetById(int id);
            int Add(string name, string club);
            void Update(Model.Opponent opponent);
            void Delete(int id);
        }
        public interface INoteRepository
        {
            IEnumerable<Model.Note> GetAllForOpponent(int opponentId);
            Model.Note? GetById(int id);
            int Add(Model.Note note);
            void Update(Model.Note note);
            void Delete(int id);
        }

        public interface IMatchRepository
        {
            IEnumerable<Model.Match> GetAllForOpponent(int opponentId);
            Model.Match? GetById(int id);
            int Add(Model.Match match);
            void Update(Model.Match match);
            void Delete(int id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Miscellaneous
{
    public class Interfaces
    {
        /// <summary>
        /// Schnittstelle für den Navigationsdienst, der die Navigation zwischen ViewModels ermöglicht.
        /// </summary>
        public interface INavigationService
        {
            void NavigateTo(object viewModel);
        }

        /// <summary>
        /// Schnittstelle für den Dialog-Manager, der das Schließen von Dialogen ermöglicht.
        /// </summary>
        public interface IDialogCloser
        {
            void Close(bool? dialogResult = true);
        }

        /// <summary>
        /// Schnittstelle für das Gegner-Repository.
        /// </summary>
        public interface IOpponentRepository
        {
            IEnumerable<Model.Opponent> GetAll();
            Model.Opponent? GetById(int id);
            int Add(string name, string club);
            void Update(Model.Opponent opponent);
            void Delete(int id);
        }

        /// <summary>
        /// Schnittstelle für das Notiz-Repository.
        /// </summary>
        public interface INoteRepository
        {
            IEnumerable<Model.Note> GetAllForOpponent(int opponentId);
            IEnumerable<Model.Note> GetAllNotes();
            Model.Note? GetById(int id);
            int Add(Model.Note note);
            void Update(Model.Note note);
            void Delete(int id);
        }

        /// <summary>
        /// Schnittstelle für das Match-Repository.
        /// </summary>
        public interface IMatchRepository
        {
            IEnumerable<Model.Match> GetAllForOpponent(int opponentId);
            IEnumerable<Model.Match> GetAllMatches();
            Model.Match? GetById(int id);
            int Add(Model.Match match);
            void Update(Model.Match match);
            void Delete(int id);
        }
    }
}

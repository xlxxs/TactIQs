using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactIQ.ViewModels;
using TactIQ.Miscellaneous;

namespace TactIQ.Miscellaneous
{
    public class MatchEditViewModelFactory : Interfaces.IMatchEditViewModelFactory
    {
        private readonly Interfaces.IMatchRepository _matchRepo;

        public MatchEditViewModelFactory(Interfaces.IMatchRepository matchRepo)
        {
            _matchRepo = matchRepo;
        }

        public MatchEditViewModel Create(Model.Match match)
        {
            return new MatchEditViewModel(_matchRepo, match);
        }
    }

    public class NoteEditViewModelFactory : Interfaces.INoteEditViewModelFactory
    {
        private readonly Interfaces.INoteRepository _noteRepo;

        public NoteEditViewModelFactory(Interfaces.INoteRepository noteRepo)
        {
            _noteRepo = noteRepo;
        }

        public NoteEditViewModel Create(Model.Note note)
        {
            return new NoteEditViewModel(_noteRepo, note);
        }
    }
}

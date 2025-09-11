using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactIQ.ViewModels;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Miscellaneous.Factories
{
    /// <summary>
    /// Factory zur Erstellung von MatchEditViewModel-Instanzen.
    /// </summary>
    public class MatchEditViewModelFactory : IMatchEditViewModelFactory
    {
        private readonly IMatchRepository _matchRepo;

        public MatchEditViewModelFactory(IMatchRepository matchRepo)
        {
            _matchRepo = matchRepo;
        }

        /// <summary>
        /// Methode zur Erstellung eines neuen MatchEditViewModel mit einem gegebenen Match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public MatchEditViewModel Create(Match match)
        {
            return new MatchEditViewModel(_matchRepo, match);
        }
    }

    /// <summary>
    /// Factory zur Erstellung von NoteEditViewModel-Instanzen.
    /// </summary>
    public class NoteEditViewModelFactory : INoteEditViewModelFactory
    {
        private readonly INoteRepository _noteRepo;

        public NoteEditViewModelFactory(INoteRepository noteRepo)
        {
            _noteRepo = noteRepo;
        }

        /// <summary>
        /// Methode zur Erstellung eines neuen NoteEditViewModel mit einer gegebenen Notiz.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public NoteEditViewModel Create(Note note)
        {
            return new NoteEditViewModel(_noteRepo, note);
        }
    }

    /// <summary>
    /// Factory zur Erstellung von ProfileEditViewModel-Instanzen.
    /// </summary>
    public class ProfileEditViewModelFactory : IProfileEditViewModelFactory
    {
        private readonly INavigationService _nav;
        private readonly IMatchEditViewModelFactory _matchEditVmFactory;
        private readonly INoteEditViewModelFactory _noteEditVmFactory;
        private readonly IOpponentProfilesViewModelFactory _opponentProfilesVmFactory;
        private readonly IOpponentRepository _opponentRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly INoteRepository _noteRepo;

        public ProfileEditViewModelFactory(
            INavigationService nav,
            IMatchEditViewModelFactory matchEditVmFactory,
            INoteEditViewModelFactory noteEditVmFactory,
            IOpponentProfilesViewModelFactory opponentProfilesVmFactory,
            IOpponentRepository opponentRepo,
            IMatchRepository matchRepo,
            INoteRepository noteRepo)
        {
            _nav = nav;
            _matchEditVmFactory = matchEditVmFactory;
            _noteEditVmFactory = noteEditVmFactory;
            _opponentProfilesVmFactory = opponentProfilesVmFactory;
            _opponentRepo = opponentRepo;
            _matchRepo = matchRepo;
            _noteRepo = noteRepo;
        }

        /// <summary>
        /// Methode zur Erstellung eines neuen ProfileEditViewModel mit einem gegebenen Gegner.
        /// </summary>
        /// <param name="opponent"></param>
        /// <returns></returns>
        public ProfileEditViewModel Create(Opponent opponent)
        {
            return new ProfileEditViewModel(_nav, _matchEditVmFactory, _noteEditVmFactory, _opponentRepo, opponent, _matchRepo, _noteRepo, _opponentProfilesVmFactory);
        }
    }

    /// <summary>
    /// Factory zur Erstellung von AnalysisViewModel-Instanzen.
    /// </summary>
    public class AnalysisViewModelFactory : IAnalysisViewModelFactory
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IOpponentRepository _opponentRepo;

        public AnalysisViewModelFactory(IMatchRepository matchRepo, IOpponentRepository opponentRepo)
        {
            _matchRepo = matchRepo;
            _opponentRepo = opponentRepo;
        }

        /// <summary>
        /// Methode zur Erstellung eines neuen AnalysisViewModel.
        /// </summary>
        /// <returns></returns>
        public AnalysisViewModel Create()
        {
            return new AnalysisViewModel(_matchRepo, _opponentRepo);
        }
    }

    /// <summary>
    /// Factory zur Erstellung von ExportViewModel-Instanzen.
    /// </summary>
    public class ExportViewModelFactory : IExportViewModelFactory
    {
        private readonly IOpponentRepository _opponentRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly INoteRepository _noteRepo;

        public ExportViewModelFactory(IOpponentRepository opponentRepo, IMatchRepository matchRepo, INoteRepository noteRepo)
        {
            _opponentRepo = opponentRepo;
            _matchRepo = matchRepo;
            _noteRepo = noteRepo;
        }

        /// <summary>
        /// Methode zur Erstellung eines neuen ExportViewModel.
        /// </summary>
        /// <returns></returns>
        public ExportViewModel Create()
        {
            return new ExportViewModel(_opponentRepo, _matchRepo, _noteRepo);
        }
    }
    /// <summary>
    /// Factory zur Erstellung von OpponentProfilesViewModel-Instanzen.
    /// </summary>
    public class OpponentProfilesViewModelFactory : IOpponentProfilesViewModelFactory
    {
        private readonly INavigationService _nav;
        private readonly IMatchEditViewModelFactory _matchEditVmFactory;
        private readonly INoteEditViewModelFactory _noteEditVmFactory;
        private readonly IProfileEditViewModelFactory _profileEditVmFactory;
        private readonly IOpponentProfilesViewModelFactory _oppomentProfileVmFactory;
        private readonly IOpponentRepository _opponentRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly INoteRepository _noteRepo;

        public OpponentProfilesViewModelFactory(
            INavigationService nav,
            IMatchEditViewModelFactory matchEditVmFactory,
            INoteEditViewModelFactory noteEditVmFactory,
            IProfileEditViewModelFactory profileEditVmFactory,
            IOpponentRepository opponentRepo,
            IMatchRepository matchRepo,
            INoteRepository noteRepo,
            IOpponentProfilesViewModelFactory opponentProfilesViewModelFactory)
        {
            _nav = nav;
            _matchEditVmFactory = matchEditVmFactory;
            _noteEditVmFactory = noteEditVmFactory;
            _profileEditVmFactory = profileEditVmFactory;
            _oppomentProfileVmFactory = opponentProfilesViewModelFactory;
            _opponentRepo = opponentRepo;
            _matchRepo = matchRepo;
            _noteRepo = noteRepo;
        }

        /// <summary>
        /// Methode zur Erstellung eines neuen OpponentProfilesViewModel.
        /// </summary>
        /// <returns></returns>
        public OpponentProfilesViewModel Create()
        {
            return new OpponentProfilesViewModel(_nav, _matchEditVmFactory, _noteEditVmFactory, _profileEditVmFactory, _opponentRepo, _matchRepo, _noteRepo, _oppomentProfileVmFactory);
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TactIQ.ViewModels;

namespace TactIQ.Views
{
    /// <summary>
    /// Interaktionslogik für ProfileEditUC.xaml
    /// </summary>
    public partial class ProfileEditUC : UserControl
    {
        public ProfileEditUC()
        {
            InitializeComponent();
        }

        private void NewMatch_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm)
            {
                var vm = new MatchEditViewModel(parentVm._nav, parentVm._matchRepo, new Model.Match { Date=DateTime.Now, OpponentId = parentVm.Id});

                var popup = new NewMatchWindow(parentVm._nav, parentVm._matchRepo, new Model.Match { Date=DateTime.Now, OpponentId = parentVm.Id}); 
                popup.Owner = Window.GetWindow(this);
                if (popup.ShowDialog() == true)
                {
                    var match = popup.NewMatch;
                    MatchDataGrid.Items.Add(match);
                }
            }
        }

        private void NewNote_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm)
            {
                var vm = new NoteEditViewModel(parentVm._nav, parentVm._notesRepo, new Model.Note { OpponentId = parentVm.Id });

                var popup = new NewNoteWindow(parentVm._nav, parentVm._notesRepo, new Model.Note { OpponentId = parentVm.Id });
                popup.Owner = Window.GetWindow(this);
                if (popup.ShowDialog() == true)
                {
                    var note = popup.NewNote;
                    NotesDataGrid.Items.Add(note);
                }
            }
        }

        private void NoteDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm && parentVm.SelectedNote != null)
            {
                var vm = new NoteEditViewModel(parentVm._nav, parentVm._notesRepo, parentVm.SelectedNote);

                var popup = new NewNoteWindow(parentVm._nav, parentVm._notesRepo, parentVm.SelectedNote);
                popup.Owner = Window.GetWindow(this);

                if (popup.ShowDialog() == true)
                {
                    parentVm.LoadNotesCommand.Execute(null);
                }
            }
        }

        private void MatchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm && parentVm.SelectedMatch != null)
            {
                var vm = new MatchEditViewModel(parentVm._nav, parentVm._matchRepo, parentVm.SelectedMatch);

                var popup = new NewMatchWindow(parentVm._nav, parentVm._matchRepo, parentVm.SelectedMatch);
                popup.Owner = Window.GetWindow(this);

                if (popup.ShowDialog() == true)
                {
                    parentVm.LoadMatchesCommand.Execute(null);
                }
            }
        }
    }
}

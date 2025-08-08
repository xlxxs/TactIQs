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
                var vm = new MatchEditViewModel(parentVm._matchRepo, new Model.Match { Date=DateTime.Now, OpponentId = parentVm.Id});
                vm.OnSaved = () => parentVm.LoadMatchesCommand.Execute(null);

                var popup = new NewMatchWindow(vm); 
                popup.Owner = Window.GetWindow(this);

                popup.ShowDialog();
            }
        }

        private void NewNote_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm)
            {
                var vm = new NoteEditViewModel(parentVm._notesRepo, new Model.Note { OpponentId = parentVm.Id });

                vm.OnSaved = () => parentVm.LoadNotesCommand.Execute(null);

                var popup = new NewNoteWindow(vm);
                popup.Owner = Window.GetWindow(this);
                popup.ShowDialog();
            }
        }


        private void NoteDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm && parentVm.SelectedNote != null)
            {
                var vm = new NoteEditViewModel(parentVm._notesRepo, parentVm.SelectedNote);

                var popup = new NewNoteWindow(vm);
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
                var vm = new MatchEditViewModel(parentVm._matchRepo, parentVm.SelectedMatch);

                var popup = new NewMatchWindow(vm);
                popup.Owner = Window.GetWindow(this);

                if (popup.ShowDialog() == true)
                {
                    parentVm.LoadMatchesCommand.Execute(null);
                }
            }
        }
    }
}

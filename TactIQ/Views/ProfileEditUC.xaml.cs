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
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Views
{
    /// <summary>
    /// Interaktionslogik für ProfileEditUC.xaml
    /// </summary>
    public partial class ProfileEditUC : UserControl
    {
        private readonly IMatchEditViewModelFactory _matchEditVmFactory;

        public ProfileEditUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Ereignis, wenn der Benutzer auf "Neues Match" klickt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewMatch_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm)
            {
                parentVm.OpenMatchEdit(this, false);
            }
        }

        /// <summary>
        /// Ereignis, wenn der Benutzer auf "Neue Notiz" klickt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewNote_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm)
            {
                parentVm.OpenNoteEdit(this, false);
            }
        }

        /// <summary>
        /// Ereignis, wenn der Benutzer eine Notiz doppelt anklickt, um sie zu bearbeiten.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoteDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm && parentVm.SelectedNote != null)
            {
                parentVm.OpenNoteEdit(this);
            }
        }

        /// <summary>
        /// Ereignis, wenn der Benutzer ein Match doppelt anklickt, um es zu bearbeiten.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MatchDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ProfileEditViewModel parentVm && parentVm.SelectedMatch != null)
            {
                parentVm.OpenMatchEdit(this);
            }
        }
    }
}

using System;
using TactIQ.Miscellaneous;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Services
{
    /// <summary>
    /// Klasse, die den Navigationsdienst implementiert.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly Action<object> _navigate;

        /// <summary>
        /// Konstruktor, der eine Navigationsaktion entgegennimmt.
        /// </summary>
        /// <param name="navigate"></param>
        public NavigationService(Action<object> navigate) => _navigate = navigate;

        /// <summary>
        /// Methode, die die Navigation zu einem bestimmten ViewModel ermöglicht.
        /// </summary>
        /// <param name="viewModel"></param>
        public void NavigateTo(object viewModel) => _navigate(viewModel);
    }
}

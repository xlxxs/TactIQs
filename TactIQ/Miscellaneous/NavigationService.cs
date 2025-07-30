using System;
using TactIQ.Miscellaneous;
using static TactIQ.Miscellaneous.Abstractions;

namespace TactIQ.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Action<object> _navigate;
        public NavigationService(Action<object> navigate) => _navigate = navigate;
        public void NavigateTo(object viewModel) => _navigate(viewModel);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Miscellaneous
{
    public class Abstractions
    {
        public interface INavigationService
        {
            void NavigateTo(object viewModel);
        }

        public interface IOpponentRepository
        {
            IEnumerable<Model.Opponent> GetAll();
            Model.Opponent? GetById(int id);
            int Add(string name, string club);
            void Update(Model.Opponent opponent);
            void Delete(int id);
        }
    }
}

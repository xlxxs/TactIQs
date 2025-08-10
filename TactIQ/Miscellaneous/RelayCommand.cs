using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TactIQ.Miscellaneous
{
    /// <summary>
    /// Klasse, die ICommand implementiert und eine Möglichkeit bietet, Befehle zu erstellen.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        /// <summary>
        /// Konstruktor, der eine Ausführungsaktion und eine optionale Bedingung für die Ausführbarkeit des Befehls entgegennimmt.
        /// </summary>
        /// <param name="execute">auszuführende Aktion</param>
        /// <param name="canExecute">Parameter zur Ausführbarkeit der Aktion</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        { _execute = execute; _canExecute = canExecute; }

        /// <summary>
        /// Methode, die angibt, ob der Befehl ausgeführt werden kann.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        /// <summary>
        /// Methode, die den Befehl ausführt.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object? parameter) => _execute(parameter);

        /// <summary>
        /// Ereignis, das ausgelöst wird, wenn sich die Ausführbarkeit des Befehls ändert.
        /// </summary>
        public event EventHandler? CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }
}

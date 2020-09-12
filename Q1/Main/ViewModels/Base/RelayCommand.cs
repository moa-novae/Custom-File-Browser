using System;
using System.Windows.Input;

namespace Q1
{
    /// <summary>
    /// A basic command that runs an Action
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Readonly Members
        /// <summary>
        /// The action to run
        /// </summary>
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new NullReferenceException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }
        // For when nothing passed to canExecute
        public RelayCommand(Action<object> execute) : this(execute, null)
        {

        }
        #endregion

        #region Public Events

        /// <summary>
        /// Default constructor
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion


        #region Command Methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// Execute the command action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }
        #endregion
    }
}

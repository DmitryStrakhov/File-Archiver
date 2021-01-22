using System;
using System.Windows.Input;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.ViewModel {
    public sealed class Command : ICommand {
        readonly Func<bool> canExecuteFunc;
        readonly Action action;

        public Command(Action action, Func<bool> canExecuteFunc = null) {
            Guard.IsNotNull(action, nameof(action));
            this.action = action;
            this.canExecuteFunc = canExecuteFunc;
        }

        event EventHandler CanExecuteChangedHandler;
        public void RaiseCanExecuteChanged() {
            CanExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand

        void ICommand.Execute(object param) => action();
        bool ICommand.CanExecute(object param) {
            if(canExecuteFunc == null) return true;
            return canExecuteFunc();
        }
        event EventHandler ICommand.CanExecuteChanged {
            add { CanExecuteChangedHandler += value; }
            remove { CanExecuteChangedHandler -= value; }
        }

        #endregion
    }
}

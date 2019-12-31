using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FileArchiver.Base;

namespace FileArchiver.ViewModel {
    public sealed class SimpleCommand : ICommand {
        readonly Action action;
        readonly Func<bool> canExecuteFunc;

        public SimpleCommand(Action action, Func<bool> canExecuteFunc = null) {
            Guard.IsNotNull(action, nameof(action));
            this.action = action;
            this.canExecuteFunc = canExecuteFunc;
        }

        event EventHandler canExecuteChangedHandler;
        public void RaiseCanExecuteChanged() {
            canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand

        void ICommand.Execute(object param) {
            action();
        }
        bool ICommand.CanExecute(object param) {
            if(canExecuteFunc == null) return true;
            return canExecuteFunc();
        }
        event EventHandler ICommand.CanExecuteChanged {
            add { canExecuteChangedHandler += value; }
            remove { canExecuteChangedHandler -= value; }
        }

        #endregion
    }


    public sealed class SimpleCommand<T> : ICommand {
        readonly Action<T> action;
        readonly Func<bool> canExecuteFunc;

        public SimpleCommand(Action<T> action, Func<bool> canExecuteFunc = null) {
            Guard.IsNotNull(action, nameof(action));
            this.action = action;
            this.canExecuteFunc = canExecuteFunc;
        }

        event EventHandler canExecuteChangedHandler;
        public void RaiseCanExecuteChanged() {
            canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand

        void ICommand.Execute(object param) {
            action((T)param);
        }
        bool ICommand.CanExecute(object param) {
            if(canExecuteFunc == null) return true;
            return canExecuteFunc();
        }
        event EventHandler ICommand.CanExecuteChanged {
            add { canExecuteChangedHandler += value; }
            remove { canExecuteChangedHandler -= value; }
        }

        #endregion
    }
}

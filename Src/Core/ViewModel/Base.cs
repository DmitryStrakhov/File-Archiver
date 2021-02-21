using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileArchiver.Core.ViewModel {

    public abstract class ViewModelBase : INotifyPropertyChanged {
        protected ViewModelBase() {
        }

        #region INotifyPropertyChanged

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

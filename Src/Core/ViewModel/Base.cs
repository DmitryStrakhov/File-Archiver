using System;
using System.ComponentModel;

namespace FileArchiver.Core.ViewModel {

    public abstract class ViewModelBase : INotifyPropertyChanged {
        protected ViewModelBase() {
        }

        #region INotifyPropertyChanged

        protected void RaisePropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

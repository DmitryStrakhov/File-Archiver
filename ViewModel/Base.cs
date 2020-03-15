using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileArchiver.ViewModel {

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

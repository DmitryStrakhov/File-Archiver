using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileArchiver.Base;

namespace FileArchiver.ViewModel {
    public sealed class MainViewModel : ViewModelBase {
        string status;
        string path;
        double progressValue;

        public MainViewModel() {
            this.status = "(status)";
            this.path = "(path)";
            this.progressValue = 0;
            this.ChooseCommand = new SimpleCommand(Choose, CanChoose);
            this.RunCommand = new SimpleCommand(Run, CanRun);
        }

        public string Path {
            get { return path; }
            set {
                if(Path == value) return;
                path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }
        public string Status {
            get { return status; }
            set {
                if(Status == value) return;
                status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }
        public double ProgressValue {
            get { return progressValue; }
            set {
                if(MathHelper.AreEqual(ProgressValue, value)) return;
                progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }

        public bool IsPathEnabled {
            get { return false; }
        }
        public bool IsStatusEnabled {
            get { return false; }
        }
        public bool IsProgressEnabled {
            get { return false; }
        }

        public void Choose() {

        }
        public void Run() {

        }

        public bool CanChoose() {
            return true;
        }
        public bool CanRun() {
            return false;
        }

        public SimpleCommand ChooseCommand { get; }
        public SimpleCommand RunCommand { get; }
    }
}

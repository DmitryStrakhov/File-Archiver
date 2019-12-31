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
            this.RunCommand = new SimpleCommand(Run, CanRun);
            this.OpenFileCommand = new SimpleCommand<string>(OpenFile);
            this.OpenFolderCommand = new SimpleCommand<string>(OpenFolder);
        }

        public string Path {
            get { return path; }
            set {
                if(Path == value) return;
                path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }
        public bool IsPathEnabled {
            get { return false; }
        }

        public string Status {
            get { return status; }
            set {
                if(Status == value) return;
                status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }
        public bool IsStatusEnabled {
            get { return false; }
        }
        public double ProgressValue {
            get { return progressValue; }
            set {
                if(MathHelper.AreEqual(ProgressValue, value)) return;
                progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }
        public bool IsProgressEnabled {
            get { return false; }
        }

        public void Run() {

        }
        public bool CanRun() {
            return true;
        }

        public void OpenFile(string path) {

        }
        public void OpenFolder(string path) {

        }

        public SimpleCommand<string> OpenFileCommand { get; }
        public SimpleCommand<string> OpenFolderCommand { get; }
        public SimpleCommand RunCommand { get; }
    }
}

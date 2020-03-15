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
            this.RunCommand = new Command(Run, CanRun);
            this.OpenFileCommand = new Command<string>(OpenFile);
            this.OpenFolderCommand = new Command<string>(OpenFolder);
        }

        public Command<string> OpenFolderCommand { get; }
        public Command<string> OpenFileCommand { get; }
        public Command RunCommand { get; }

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
        public bool IsChoiceButtonEnabled {
            get { return true; }
        }
        public bool IsRunButtonEnabled {
            get { return false; }
        }

        private void Run() {
        }
        private bool CanRun() {
            return true;
        }

        private void OpenFolder(string folderPath) {
        }
        private void OpenFile(string filePath) {
        }
    }
}
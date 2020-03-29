using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileArchiver.Base;
using FileArchiver.Helpers;

namespace FileArchiver.ViewModel {
    public class MainViewModel : ViewModelBase {
        readonly ServiceFactory serviceFactory;
        string status;
        string path;
        double progressValue;

        public MainViewModel(ServiceFactory serviceFactory) {
            Guard.IsNotNull(serviceFactory, nameof(serviceFactory));
            this.serviceFactory = serviceFactory;
            this.status = string.Empty;
            this.path = string.Empty;
            this.progressValue = 0;
            this.RunCommand = new Command(Run, CanRun);
        }

        public Command RunCommand { get; }

        public string Path {
            get { return path; }
            set {
                if(Path == value) return;
                path = value;
                RaisePropertyChanged(nameof(Path));
                RunCommand.RaiseCanExecuteChanged();
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

        internal bool CanRun() {
            InputType inputType = serviceFactory.InputDataService.GetInputType(path);
            return inputType != InputType.Unknown;
        }
        internal virtual void Run() {
            InputType inputType = serviceFactory.InputDataService.GetInputType(path);
            switch(inputType) {
                case InputType.FileToEncode:
                    EncodeFile();
                    break;
                case InputType.FolderToEncode:
                    EncodeFolder();
                    break;
                case InputType.Archive:
                    DecodeArchive();
                    break;
                default:
                    throw new Exception(nameof(inputType));
            }
        }

        private void EncodeFile() {
            string targetPath = serviceFactory.FileSelectorService.GetSaveFile();
            serviceFactory.EncodingService.EncodeFile(Path, targetPath);
        }
        private void EncodeFolder() {
            string targetFile = serviceFactory.FileSelectorService.GetSaveFile();
            serviceFactory.EncodingService.EncodeFolder(Path, targetFile);
        }
        private void DecodeArchive() {
            string targetFolder = serviceFactory.FolderSelectorService.GetFolder();
            serviceFactory.DecodingService.Decode(Path, targetFolder);
        }
    }
}
using System;
using System.Threading.Tasks;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.ViewModel {
    public class MainViewModel : ViewModelBase {
        string statusMessage;
        string path;
        double progressValue;

        public MainViewModel() {
            this.statusMessage = "[status]";
            this.path = string.Empty;
            this.progressValue = 0;
            this.RunCommand = new Command(async () => await Run(), CanRun);
        }

        public Command RunCommand { get; }
        public IFileSelectorService FileSelectorService { get; set; }
        public IFolderSelectorService FolderSelectorService { get; set; }
        public IInputDataService InputDataService { get; set; }
        public IHuffmanEncodingService EncodingService { get; set; }
        public IHuffmanDecodingService DecodingService { get; set; }

        public string Path {
            get { return path; }
            set {
                if(Path == value) return;
                path = value;
                RaisePropertyChanged(nameof(Path));
                RunCommand.RaiseCanExecuteChanged();
            }
        }
        public string StatusMessage {
            get { return statusMessage; }
            set {
                if(StatusMessage == value) return;
                statusMessage = value;
                RaisePropertyChanged(nameof(StatusMessage));
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
        public bool IsChoiceButtonEnabled {
            get { return true; }
        }

        public bool CanRun() {
            InputCommand inputCommand = InputDataService.GetInputCommand(path);
            return inputCommand != InputCommand.Unknown;
        }
        public virtual async Task Run() {
            ProgressValue = 0;
            InputCommand inputCommand = InputDataService.GetInputCommand(path);
            switch(inputCommand) {
                case InputCommand.Encode:
                    await Encode();
                    break;
                case InputCommand.Decode:
                    await Decode();
                    break;
                default:
                    throw new Exception(nameof(inputCommand));
            }
        }

        private async Task Encode() {
            string targetPath = FileSelectorService.GetSaveFile();
            if(string.IsNullOrEmpty(targetPath)) return;

            await EncodingService.EncodeAsync(Path, targetPath, new DefaultProgressHandler(this));
        }
        private async Task Decode() {
            string targetFolder = FolderSelectorService.GetFolder();
            if(string.IsNullOrEmpty(targetFolder)) return;

            await DecodingService.DecodeAsync(Path, targetFolder, new DefaultProgressHandler(this));
        }
    }
}
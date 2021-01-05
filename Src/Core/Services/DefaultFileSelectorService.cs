using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Services {
    public class DefaultFileSelectorService : IFileSelectorService {
        public string GetSaveFile() {
            return FileDialogHelper.SaveFile();
        }
    }
}
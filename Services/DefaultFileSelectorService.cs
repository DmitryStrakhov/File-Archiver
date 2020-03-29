using System;
using FileArchiver.Base;
using FileArchiver.Helpers;

namespace FileArchiver.Services {
    public class DefaultFileSelectorService : IFileSelectorService {
        public string GetSaveFile() {
            return FileDialogHelper.SaveFile();
        }
    }
}
using System;
using FileArchiver.Base;
using FileArchiver.Helpers;

namespace FileArchiver.Services {
    public class DefaultFolderSelectorService : IFolderSelectorService {
        public string GetFolder() {
            return FolderDialogHelper.Show();
        }
    }
}
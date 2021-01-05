using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Services {
    public class DefaultFolderSelectorService : IFolderSelectorService {
        public string GetFolder() {
            return FolderDialogHelper.Show();
        }
    }
}
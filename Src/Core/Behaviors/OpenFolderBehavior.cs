using System;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Behaviors {
    public sealed class OpenFolderBehavior : FileSystemBehavior {
        public OpenFolderBehavior() {
        }
        protected override void HandleMenuItemClick() {
            string folderPath = FolderDialogHelper.Show();
            if(!string.IsNullOrEmpty(folderPath)) ExecuteCommand(folderPath);
        }
        protected override string CommandName { get { return "Open Folder ..."; } }
    }
}

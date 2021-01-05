using System;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Behaviors {
    public sealed class OpenFileBehavior : FileSystemBehavior {
        public OpenFileBehavior() {
        }
        protected override void HandleMenuItemClick() {
            string filePath = FileDialogHelper.OpenFile();
            if(!string.IsNullOrEmpty(filePath)) ExecuteCommand(filePath);
        }
        protected override string CommandName { get { return "Open File ..."; } }
    }
}

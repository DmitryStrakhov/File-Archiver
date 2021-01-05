using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FileArchiver.Helpers;

namespace FileArchiver.Behaviors {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DialogResult = System.Windows.Forms.DialogResult;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;

namespace FileArchiver.Behaviors {
    public sealed class OpenFolderBehavior : FileSystemBehavior {
        public OpenFolderBehavior() {
        }
        protected override void HandleMenuItemClick() {
            using(FolderBrowserDialog dialog = new FolderBrowserDialog()) {
                if(dialog.ShowDialog() == DialogResult.OK) ExecuteCommand(dialog.SelectedPath);
            }
        }
        protected override string CommandName { get { return "Open Folder ..."; } }
    }
}

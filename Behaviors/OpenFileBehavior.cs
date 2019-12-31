using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DialogResult = System.Windows.Forms.DialogResult;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace FileArchiver.Behaviors {
    public sealed class OpenFileBehavior : FileSystemBehavior {
        public OpenFileBehavior() {
        }
        protected override void HandleMenuItemClick() {
            using(OpenFileDialog dialog = new OpenFileDialog()) {
                if(dialog.ShowDialog() == DialogResult.OK) ExecuteCommand(dialog.FileName);
            }
        }
        protected override string CommandName { get { return "Open File ..."; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileArchiver.Helpers;

namespace FileArchiver.Behaviors {
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

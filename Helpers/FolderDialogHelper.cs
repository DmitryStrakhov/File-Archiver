using System;
using System.Windows.Forms;

namespace FileArchiver.Helpers {
    public static class FolderDialogHelper {
        public static string Show() {
            using(FolderBrowserDialog dialog = new FolderBrowserDialog()) {
                if(dialog.ShowDialog() == DialogResult.OK) return dialog.SelectedPath;
            }
            return null;
        }
    }
}
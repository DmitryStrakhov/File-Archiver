using System;
using System.Windows.Forms;

namespace FileArchiver.Core.Helpers {
    public static class FileDialogHelper {
        public static string OpenFile() {
            using(OpenFileDialog dialog = new OpenFileDialog()) {
                if(dialog.ShowDialog() == DialogResult.OK) return dialog.FileName;
            }
            return null;
        }
        public static string SaveFile(string defaultExtension) {
            using(SaveFileDialog dialog = new SaveFileDialog()) {
                dialog.DefaultExt = defaultExtension;
                if(dialog.ShowDialog() == DialogResult.OK) return dialog.FileName;
            }
            return null;
        }
    }
}
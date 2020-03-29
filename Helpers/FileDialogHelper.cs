using System;
using System.Windows.Forms;

namespace FileArchiver.Helpers {
    public static class FileDialogHelper {
        public static string OpenFile() {
            using(OpenFileDialog dialog = new OpenFileDialog()) {
                if(dialog.ShowDialog() == DialogResult.OK) return dialog.FileName;
            }
            return null;
        }
        public static string SaveFile() {
            using(SaveFileDialog dialog = new SaveFileDialog()) {
                if(dialog.ShowDialog() == DialogResult.OK) return dialog.FileName;
            }
            return null;
        }
    }
}
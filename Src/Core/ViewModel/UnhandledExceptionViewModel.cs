using System;
using System.Text;
using System.Windows;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.ViewModel {
    public class UnhandledExceptionViewModel {
        readonly Exception exception;
        readonly Lazy<string> exceptionNameLazy;
        readonly Lazy<string> exceptionDetailsLazy;

        public UnhandledExceptionViewModel(Exception e) {
            Guard.IsNotNull(e, nameof(e));
            this.exception = e;

            this.exceptionNameLazy = new Lazy<string>(()
                => exception.GetType().Name);

            this.exceptionDetailsLazy = new Lazy<string>(() => {
                StringBuilder stringBuilder = new StringBuilder(2048);

                Exception ex = exception;
                do {
                    stringBuilder.Append("Message: ");
                    stringBuilder.AppendLine(ex.Message);
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("Call stack:");
                    stringBuilder.AppendLine(ex.StackTrace);
                    ex = ex.InnerException;
                    if(ex != null) {
                        stringBuilder.AppendLine();
                        stringBuilder.Append("Inner Exception: ");
                        stringBuilder.AppendLine(ex.GetType().Name);
                        stringBuilder.AppendLine();
                    }
                }
                while(ex != null);
                return stringBuilder.ToString();
            });
            this.CopyCommand = new Command(Copy);
        }
        public Command CopyCommand { get; }

        public string ExceptionName {
            get { return exceptionNameLazy.Value; }
        }
        public string ExceptionDetails {
            get { return exceptionDetailsLazy.Value; }
        }

        private void Copy() {
            try { Clipboard.SetText(PrepareClipboardText(), TextDataFormat.UnicodeText); }
            catch(Exception) { /*ignored*/ }
        }
        private string PrepareClipboardText() {
            return ExceptionName + Environment.NewLine + ExceptionDetails;
        }
    }
}
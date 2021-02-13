using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.ViewModel {
    public class DefaultProgressHandler : IProgress<CodingProgressInfo> {
        readonly MainViewModel viewModel;

        public DefaultProgressHandler(MainViewModel viewModel) {
            Guard.IsNotNull(viewModel, nameof(this.viewModel));
            this.viewModel = viewModel;
        }

        public void Report(CodingProgressInfo value) {
            viewModel.ProgressValue = value.Value;
            viewModel.StatusMessage = value.StatusMessage;
        }
    }
}
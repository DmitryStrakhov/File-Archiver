using System;
using System.IO;
using FileArchiver.Base;
using FileArchiver.Helpers;

namespace FileArchiver.Services {
    public class DefaultInputDataService : IInputDataService {
        readonly IPlatformService platform;

        public DefaultInputDataService(IPlatformService platform) {
            Guard.IsNotNull(platform, nameof(platform));
            this.platform = platform;
        }
        public InputType GetInputType(string path) {
            if(!string.IsNullOrEmpty(path)) {
                if(platform.FolderExists(path)) return InputType.FolderToEncode;

                if(platform.FileExists(path))
                    return IsArchiveExtension(Path.GetExtension(path)) ? InputType.Archive : InputType.FileToEncode;
            }
            return InputType.Unknown;
        }
        static bool IsArchiveExtension(string extension) {
            return string.Equals(extension, ".archive", StringComparison.OrdinalIgnoreCase);
        }
    }
}
using System;
using System.IO;
using FileArchiver.Core.Base;
using FileArchiver.Core.Helpers;

namespace FileArchiver.Core.Services {
    public class DefaultInputDataService : IInputDataService {
        readonly IPlatformService platform;

        public DefaultInputDataService(IPlatformService platform) {
            Guard.IsNotNull(platform, nameof(platform));
            this.platform = platform;
        }
        public InputCommand GetInputCommand(string path) {
            if(platform.IsPathExists(path)) {
                if(StringHelper.AreEqual(Path.GetExtension(path), ".archive")) return InputCommand.Decode;
                return InputCommand.Encode;
            }
            return InputCommand.Unknown;
        }
    }
}
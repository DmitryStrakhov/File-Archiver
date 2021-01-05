using System;

namespace FileArchiver.Core.Base {
    public enum InputCommand {
        Encode,
        Decode,
        Unknown
    }

    public interface IInputDataService {
        InputCommand GetInputCommand(string path);
    }
}
namespace FileArchiver.Base {
    public enum InputCommand {
        Encode,
        Decode,
        Unknown
    }

    public interface IInputDataService {
        InputCommand GetInputCommand(string path);
    }
}
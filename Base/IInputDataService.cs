namespace FileArchiver.Base {
    public enum InputType {
        FileToEncode,
        FolderToEncode,
        Archive,
        Unknown
    }

    public interface IInputDataService {
        InputType GetInputType(string path);
    }
}
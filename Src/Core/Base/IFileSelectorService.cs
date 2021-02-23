using System;

namespace FileArchiver.Core.Base {
    public interface IFileSelectorService {
        string GetSaveFile(string defaultExtension);
    }
}
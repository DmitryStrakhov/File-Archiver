namespace FileArchiver.Base {
    public abstract class ServiceFactory {
        public abstract IFileSelectorService FileSelectorService { get; }
        public abstract IFolderSelectorService FolderSelectorService { get; }
        public abstract IInputDataService InputDataService { get; }
        public abstract IHuffmanEncodingService EncodingService { get; }
        public abstract IHuffmanDecodingService DecodingService { get; }
    }
}
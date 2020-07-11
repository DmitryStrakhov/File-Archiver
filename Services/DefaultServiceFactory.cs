﻿using FileArchiver.Base;

namespace FileArchiver.Services {
    public class DefaultServiceFactory : ServiceFactory {
        public override IFileSelectorService FileSelectorService { get; } = new DefaultFileSelectorService();
        public override IFolderSelectorService FolderSelectorService { get; } = new DefaultFolderSelectorService();
        public override IInputDataService InputDataService { get; } = new DefaultInputDataService(new DefaultPlatformService());
        public override IHuffmanEncodingService EncodingService { get; } = new DefaultHuffmanEncodingService(new DefaultFileSystemService(new DefaultPlatformService()), new DefaultPlatformService());
        public override IHuffmanDecodingService DecodingService { get; } = new DefaultHuffmanDecodingService(new DefaultPlatformService());
    }
}
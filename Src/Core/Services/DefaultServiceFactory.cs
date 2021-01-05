using System;
using FileArchiver.Core.Base;
using FileArchiver.Core.Builders;
using FileArchiver.Core.Parsers;

namespace FileArchiver.Core.Services {
    public class DefaultServiceFactory : ServiceFactory {
        public override IFileSelectorService FileSelectorService { get; } = new DefaultFileSelectorService();
        public override IFolderSelectorService FolderSelectorService { get; } = new DefaultFolderSelectorService();
        public override IInputDataService InputDataService { get; } = new DefaultInputDataService(new DefaultPlatformService());
        public override IHuffmanEncodingService EncodingService { get; } = new DefaultHuffmanEncodingService(new DefaultFileSystemService(new DefaultPlatformService()), new DefaultPlatformService(), new DefaultStreamBuilder());
        public override IHuffmanDecodingService DecodingService { get; } = new DefaultHuffmanDecodingService(new DefaultPlatformService(), new DefaultStreamParser());
    }
}
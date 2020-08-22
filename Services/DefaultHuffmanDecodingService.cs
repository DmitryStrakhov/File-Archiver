using System;
using System.IO;
using FileArchiver.Base;
using FileArchiver.FileCore;
using FileArchiver.Helpers;
using FileArchiver.HuffmanCore;

namespace FileArchiver.Services {
    public class DefaultHuffmanDecodingService : IHuffmanDecodingService {
        readonly IPlatformService platform;

        public DefaultHuffmanDecodingService(IPlatformService platform) {
            Guard.IsNotNull(platform, nameof(platform));
            this.platform = platform;
        }

        public bool Decode(string inputFile, string outputFolder) {
            //FileDecodingInputStream inputStream = new FileDecodingInputStream(inputFile, platform);
            //FileDecodingOutputStream outputStream = new FileDecodingOutputStream(GetOutputFile(inputFile, outputFolder), platform);
            //
            //try {
            //    inputStream.BeginRead();
            //    new HuffmanDecoder().Decode(inputStream, SharedTree.Instance.CastTo<HuffmanTree>(), outputStream);
            //    inputStream.EndRead();
            //}
            //finally {
            //    inputStream.Dispose();
            //    outputStream.Dispose();
            //}

            return true;
        }
        //private static string GetOutputFile(string inputFile, string outputFolder) {
        //    string fileName = Path.GetFileNameWithoutExtension(inputFile);
        //    return Path.Combine(outputFolder, fileName);
        //}
    }
}
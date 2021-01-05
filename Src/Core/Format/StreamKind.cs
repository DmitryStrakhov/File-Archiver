using System;

namespace FileArchiver.Core.Format {
    public enum StreamKind : byte {
        FS_CODE = 0x0,
        DS_CODE = 0x1,
        WT_CODE = 0x2
    }
}
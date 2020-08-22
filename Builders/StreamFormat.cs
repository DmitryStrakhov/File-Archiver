using System;

namespace FileArchiver.Builders {
    public enum StreamFormat : byte {
        FS_CODE = 0x0,
        DS_CODE = 0x1,
        WT_CODE = 0x2
    }
}
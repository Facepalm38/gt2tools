﻿namespace GT1.ArchiveExtractor
{
    public interface IFileWriter
    {
        void CreateDirectory(string path);

        void Write(string path, byte[] contents);
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StreamExtensions;

namespace GT3.VOLExtractor
{
    public class DirectoryEntry : Entry
    {
        public const uint Flag = 0x01000000;

        public List<Entry> Entries { get; set; }

        public DirectoryEntry ParentDirectory { get; set; }

        public override void Read(Stream stream)
        {
            //Console.WriteLine($"{stream.Position}");
            uint filenamePosition = stream.ReadUInt() - Flag;
            Name = Program.GetFilename(filenamePosition);
            uint numberOfEntries = stream.ReadUInt();
            Entries = new List<Entry>((int)numberOfEntries);

            for (int i = 0; i < numberOfEntries; i++)
            {
                uint entryPosition = stream.ReadUInt();
                long currentPosition = stream.Position;
                if (entryPosition < currentPosition)
                {
                    Entries.Add(new DirectoryEntry { Name = ".." });
                    continue;
                }

                stream.Position = entryPosition;
                Entries.Add(Create(stream.ReadUInt()));
                stream.Position -= 4;
                Entries[i].Read(stream);
                stream.Position = currentPosition;
            }
        }

        public override void Extract(string path, Stream stream)
        {
            path = Path.Combine(path, Name);
            Console.WriteLine($"Extracting directory: {path}");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (var entry in Entries)
            {
                if (entry.Name != "..")
                {
                    entry.Extract(path, stream);
                }
            }
        }

        public override void Import(string path)
        {
            //Console.WriteLine(path);
            Name = Path.GetFileName(path);
            List<string> childPaths = Directory.EnumerateFileSystemEntries(path).ToList();
            childPaths.Sort(StringComparer.Ordinal);

            Entries = new List<Entry>();
            Entries.Add(new DirectoryEntry { Name = ".." });

            foreach (string childPath in childPaths)
            {
                Entry entry;
                if ((File.GetAttributes(childPath) & FileAttributes.Directory) != 0)
                {
                    entry = new DirectoryEntry { ParentDirectory = this };
                }
                else if (ArchiveEntry.FileExtensions.Contains(Path.GetExtension(childPath)))
                {
                    entry = new ArchiveEntry();
                }
                else
                {
                    entry = new FileEntry();
                }
                entry.Import(childPath);
                Entries.Add(entry);
            }
        }

        public override void AllocateHeaderSpace(Stream stream)
        {
            HeaderPosition = stream.Position;
            //Console.WriteLine($"{HeaderPosition}");
            stream.Position += 8;
            stream.Position += 4 * Entries.Count;
            Entries[0].HeaderPosition = ParentDirectory?.HeaderPosition ?? 0;
        }

        public override uint GetFlag() => Flag;

        public override void Write(Stream stream)
        {
            Console.WriteLine($"Importing directory: {Name}");
            uint currentPosition = (uint)stream.Position;
            stream.Position = HeaderPosition + 4;
            stream.WriteUInt((uint)Entries.Count);
            foreach (var entry in Entries)
            {
                stream.WriteUInt((uint)entry.HeaderPosition);
            }
            stream.Position = currentPosition;
        }
    }
}
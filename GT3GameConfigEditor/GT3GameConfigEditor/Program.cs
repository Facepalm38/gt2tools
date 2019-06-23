﻿using System.IO;
using StreamExtensions;

namespace GT3.GameConfigEditor
{
    class Program
    {
        private enum ListType : byte {
            Unknown00,
            Unknown01,
            Unknown02,
            DemoDemos,
            Events,
            Unknown05,
            Courses,
            DemoCarClasses,
            TrackAvailability,
            DemoCarUnknown,
            GTAutoPrices,
            Unknown0B,
            Demos,
            GhostsMaybe,
            CarClasses,
            Prizes
        }

        static void Main(string[] args)
        {
            string filename = args[0];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                string directory = Path.GetFileNameWithoutExtension(filename);
                Directory.CreateDirectory(directory);
                uint unknown = file.ReadUInt();
                uint unknown2 = file.ReadUInt();
                uint unknown3 = file.ReadUInt();
                uint headerSize = file.ReadUInt();

                while (file.Position < headerSize)
                {
                    uint unknown4 = file.ReadUInt();
                    uint structurePos = file.ReadUInt();
                    long currentPos = file.Position;

                    uint nextStructurePos;
                    if (file.Position < headerSize)
                    {
                        uint nextUnknown4 = file.ReadUInt();
                        nextStructurePos = file.ReadUInt();
                    }
                    else
                    {
                        nextStructurePos = (uint)file.Length;
                    }
                    
                    uint structureSize = nextStructurePos - structurePos;

                    file.Position = structurePos;
                    byte[] buffer = new byte[structureSize];
                    file.Read(buffer);

                    using (var output = new FileStream(Path.Combine(directory, $"{(ListType)unknown4}.dat"), FileMode.Create, FileAccess.Write))
                    {
                        output.Write(buffer);
                    }
                    file.Position = currentPos;
                }
            }
        }
    }
}
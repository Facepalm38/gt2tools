﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace GT2.DataSplitter
{
    using StreamExtensions;

    public class GTModeRace
    {
        public List<Event> Events { get; set; } = new List<Event>();
        public List<EnemyCars> EnemyCars { get; set; } = new List<EnemyCars>();
        public List<Regulations> Regulations { get; set; } = new List<Regulations>();
        
        public void ReadData(string filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var blocks = new List<DataBlock>();

                for (int i = 1; i <= 4; i++)
                {
                    file.Position = 8 * i;
                    uint blockStart = file.ReadUInt();
                    uint blockSize = file.ReadUInt();
                    blocks.Add(new DataBlock { BlockStart = blockStart, BlockSize = blockSize });
                }

                Events.Read(file, blocks[0].BlockStart, blocks[0].BlockSize);
                EnemyCars.Read(file, blocks[1].BlockStart, blocks[1].BlockSize);
                Regulations.Read(file, blocks[2].BlockStart, blocks[2].BlockSize);
                RaceStringTable.Read(file, blocks[3].BlockStart, blocks[3].BlockSize);
            }
        }

        public void DumpData()
        {
            Regulations.Dump();
            EnemyCars.Dump();
            Events.Dump();
        }

        public void ImportData()
        {
            Regulations.Import();
            EnemyCars.Import();
            Events.Import();
        }

        public void WriteData(string filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                file.Write(new byte[] { 0x47, 0x54, 0x44, 0x54, 0x6C, 0x00, 0x06, 0x00 }, 0, 8); // The 0x06 is the number of indices

                file.Position = (0x06 * 8) + 7;
                file.WriteByte(0x00); // Data starts at 0x38 so position EOF

                uint i = 1;
                Events.Write(file, 8 * i++);
                EnemyCars.Write(file, 8 * i++);
                Regulations.Write(file, 8 * i++);
                RaceStringTable.Write(file, 8 * i++);

                file.Position = 0;
                using (FileStream zipFile = new FileStream(filename + ".gz", FileMode.Create, FileAccess.Write))
                {
                    using (GZipStream zip = new GZipStream(zipFile, CompressionMode.Compress))
                    {
                        file.CopyTo(zip);
                    }
                }
            }
        }

        public struct DataBlock
        {
            public uint BlockStart;
            public uint BlockSize;
        }
    }
}

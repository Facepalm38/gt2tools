﻿using System.IO;

namespace GT2.ModelTool.Structures
{
    public class UVCoordinate
    {
        public byte X { get; set; }
        public byte Y { get; set; }

        public void ReadFromCDO(Stream stream)
        {
            X = (byte)stream.ReadByte();
            Y = (byte)stream.ReadByte();
        }

        public void ReadFromCAR(Stream stream)
        {
            X = (byte)stream.ReadByte();
            int y = stream.ReadByte();
            if (y >= 32)
            {
                y -= 32;
            }
            Y = (byte)y;
        }

        public void WriteToCDO(Stream stream)
        {
            stream.WriteByte(X);
            stream.WriteByte(Y);
        }
    }
}
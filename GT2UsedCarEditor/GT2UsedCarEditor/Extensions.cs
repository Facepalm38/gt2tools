﻿using System.IO;

namespace GT2UsedCarEditor
{
    public static class Extensions
    {
        public static uint ReadUInt(this Stream stream)
        {
            byte[] rawValue = new byte[4];
            stream.Read(rawValue, 0, 4);
            return (uint)(rawValue[3] * 256 * 256 * 256 + rawValue[2] * 256 * 256 + rawValue[1] * 256 + rawValue[0]);
        }

        public static uint ReadUInt(this byte[] array)
        {
            return (uint)(array[3] * 256 * 256 * 256 + array[2] * 256 * 256 + array[1] * 256 + array[0]);
        }

        public static void WriteUInt(this Stream stream, uint value)
        {
            stream.Write(value.ToByteArray(), 0, 4);
        }

        public static byte[] ToByteArray(this uint value)
        {
            byte[] byteArray = new byte[4];
            byteArray[3] = (byte)((value / 256 / 256 / 256) % 256);
            byteArray[2] = (byte)((value / 256 / 256) % 256);
            byteArray[1] = (byte)((value / 256) % 256);
            byteArray[0] = (byte)(value % 256);
            return byteArray;
        }

        public static ushort ReadUShort(this Stream stream)
        {
            byte[] rawValue = new byte[2];
            stream.Read(rawValue, 0, 2);
            return (ushort)(rawValue[1] * 256 + rawValue[0]);
        }
    }
}
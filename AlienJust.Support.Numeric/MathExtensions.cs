﻿using System;
using System.Collections.Generic;

namespace AlienJust.Support.Numeric
{
    public static class MathExtensions
    {
        #region CRC16

        private static readonly byte[] ACrcHi = {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40
        };

        private static readonly byte[] ACrcLo = {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7,
            0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E,
            0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09, 0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9,
            0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC,
            0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32,
            0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D,
            0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A, 0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38,
            0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF,
            0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1,
            0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4,
            0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F, 0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB,
            0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA,
            0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0,
            0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97,
            0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C, 0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E,
            0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89,
            0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83,
            0x41, 0x81, 0x80, 0x40
        };

        #endregion

        #region CRC8
        static byte[] tableForCrc8 = new byte[256];
        // x8 + x7 + x6 + x4 + x2 + 1
        const byte polyForCrc8 = 0xd5;



        static byte[] crc8tab = new byte[]
        {
            0,   94,  188, 226,  97,  63, 221, 131, 194, 156, 126,  32, 163, 253,  31,  65,
            157, 195,  33, 127, 252, 162,  64,  30,  95,   1, 227, 189,  62,  96, 130, 220,
            35,  125, 159, 193,  66,  28, 254, 160, 225, 191,  93,   3, 128, 222,  60,  98,
            190, 224,   2,  92, 223, 129,  99,  61, 124,  34, 192, 158,  29,  67, 161, 255,
            70,  24,  250, 164,  39, 121, 155, 197, 132, 218,  56, 102, 229, 187,  89,   7,
            219, 133, 103,  57, 186, 228,   6,  88,  25,  71, 165, 251, 120,  38, 196, 154,
            101, 59,  217, 135,   4,  90, 184, 230, 167, 249,  27,  69, 198, 152, 122,  36,
            248, 166,  68,  26, 153, 199,  37, 123,  58, 100, 134, 216,  91,   5, 231, 185,
            140, 210,  48, 110, 237, 179,  81,  15,  78,  16, 242, 172,  47, 113, 147, 205,
            17,  79,  173, 243, 112,  46, 204, 146, 211, 141, 111,  49, 178, 236,  14,  80,
            175, 241,  19,  77, 206, 144, 114,  44, 109,  51, 209, 143,  12,  82, 176, 238,
            50,  108, 142, 208,  83,  13, 239, 177, 240, 174,  76,  18, 145, 207,  45, 115,
            202, 148, 118,  40, 171, 245,  23,  73,   8,  86, 180, 234, 105,  55, 213, 139,
            87,  9,   235, 181,  54, 104, 138, 212, 149, 203,  41, 119, 244, 170,  72,  22,
            233, 183,  85,  11, 136, 214,  52, 106,  43, 117, 151, 201,  74,  20, 246, 168,
            116, 42,  200, 150,  21,  75, 169, 247, 182, 232,  10,  84, 215, 137, 107,  53
        };
        #endregion

        static MathExtensions()
        {
            // filling CRC8 table
            for (int i = 0; i < 256; ++i)
            {
                int temp = i;
                for (int j = 0; j < 8; ++j)
                {
                    if ((temp & 0x80) != 0)
                    {
                        temp = (temp << 1) ^ polyForCrc8;
                    }
                    else
                    {
                        temp <<= 1;
                    }
                }
                tableForCrc8[i] = (byte)temp;
            }
        }


        public static Crc16 GetCrc16FromIlist(IList<byte> data, int startByte, int length)
        {
            //Console.WriteLine("startbyte: " + startByte + " length: " + length);
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = startByte; i < startByte + length; ++i)
            {
                //Console.WriteLine("proceed byte " + i);
                int index = crcLo ^ data[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            return new Crc16(crcLo, crcHi);
        }
        public static Crc16 GetCrc16FromIlist(IList<byte> data)
        {
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = 0; i < data.Count; ++i)
            {
                int index = crcLo ^ data[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            return new Crc16(crcLo, crcHi);
        }



        public static Crc16 GetCrc16FromArray(byte[] data)
        {
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = 0; i < data.Length; ++i)
            {
                int index = crcLo ^ data[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            return new Crc16(crcLo, crcHi);
        }
        public static Crc16 GetCrc16FromArray(byte[] data, int startByte, int length)
        {
            //Console.WriteLine("startbyte: " + startByte + " length: " + length);
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = startByte; i < startByte + length; ++i)
            {
                //Console.WriteLine("proceed byte " + i);
                int index = crcLo ^ data[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            return new Crc16(crcLo, crcHi);
        }

        public static Crc16 GetCrc16FromList(List<byte> data, int startByte, int length)
        {
            //Console.WriteLine("startbyte: " + startByte + " length: " + length);
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = startByte; i < startByte + length; ++i)
            {
                //Console.WriteLine("proceed byte " + i);
                int index = crcLo ^ data[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            return new Crc16(crcLo, crcHi);
        }
        public static Crc16 GetCrc16FromList(List<byte> data)
        {
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = 0; i < data.Count; ++i)
            {
                int index = crcLo ^ data[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            return new Crc16(crcLo, crcHi);
        }



        private static readonly byte[] Oddparity = { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 };

        public static ushort DoCrc16(byte cdataIn, ushort utilcrc16In)
        {
            var cdata = (byte)((cdataIn ^ ((byte)(utilcrc16In & 0xFF))) & 0xFF);
            //cdata = (byte) ((cdata ^ (utilcrc16 & 0xFF)));
            //utilcrc16 >>= 8;
            var utilcrc16 = (ushort)(utilcrc16In >> 8);

            if ((Oddparity[cdata & 0x0F] ^ Oddparity[cdata >> 4]) != 0)
                utilcrc16 ^= 0xC001;

            cdata <<= 6;
            utilcrc16 ^= cdata;
            cdata <<= 1;
            utilcrc16 ^= cdata;

            return utilcrc16;
        }

        public static Crc16 Crc16ByDo(IList<byte> data, int startByte, int length)
        {
            ushort crc = 0xFFFF;
            for (int i = startByte; i < length; ++i)
            {
                crc = DoCrc16(data[i], crc);
            }
            return new Crc16((byte)(crc & 0x0FF), (byte)((crc & 0xFF00) >> 8));
        }
        public static Crc16 Crc16ByDo(IList<byte> data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Count; ++i)
            {
                crc = DoCrc16(data[i], crc);
            }
            return new Crc16((byte)(crc & 0x00FF), (byte)((crc & 0xFF00) >> 8));
        }



        public static Crc16 GetCrc16Maks(IList<byte> buffer)
        {
            ushort crc = 0xFFFF;
            for (int pos = 0; pos < buffer.Count; ++pos)
            {
                crc ^= buffer[pos];

                for (int i = 8; i != 0; --i)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;

                        crc ^= 0xA001; // Полином.
                    }

                    else crc >>= 1;
                }
            }
            return new Crc16(Convert.ToByte(crc % 0x100), Convert.ToByte(crc >> 8));
        }

        public static void FillCrc16AtTheEndOfArrayHighLow(byte[] array)
        {
            // len check skipped to increase performance
            // if (array.Length < 3) throw new Exception("Длина массива должна быть не менее трёх байт");
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = 0; i < array.Length - 2; ++i)
            {
                int index = crcLo ^ array[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            array[array.Length - 2] = crcHi;
            array[array.Length - 1] = crcLo;
        }

        public static void FillCrc16AtTheEndOfArrayLowHigh(byte[] array)
        {
            // len check skipped to increase performance
            // if (array.Length < 3) throw new Exception("Длина массива должна быть не менее трёх байт");
            byte crcHi = 0xFF;
            byte crcLo = 0xFF;
            for (int i = 0; i < array.Length - 2; ++i)
            {
                int index = crcLo ^ array[i];
                crcLo = (byte)(crcHi ^ ACrcHi[index]);
                crcHi = ACrcLo[index];
            }
            array[array.Length - 2] = crcLo;
            array[array.Length - 1] = crcHi;
        }



        public static byte GetCrc8FromArray(byte[] data, int startByte, int length)
        {
            byte crc = 0;
            for (int i = startByte; i < startByte + length; ++i)
            {
                crc = tableForCrc8[crc ^ data[i]];
            }
            return crc;
        }

        public static byte GetCrc8FromArrayRoman(byte[] data, int startByte, int length)
        {
            byte crc = 0;
            for (int i = startByte; i < startByte + length; ++i)
            {
                crc = crc8tab[crc ^ data[i]];
            }
            return crc;
        }
    }
}
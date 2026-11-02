using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Infrastructure.Payment
{
    public static class CRC32
    {
        private static readonly uint[] Table =
            CreateTable();

        public static uint Compute(byte[] bytes)
        {
            uint crc = 0xFFFFFFFF;

            foreach (byte b in bytes)
            {
                byte index =
                    (byte)(crc ^ b);

                crc =
                    (crc >> 8) ^
                    Table[index];
            }

            return crc ^ 0xFFFFFFFF;
        }

        private static uint[] CreateTable()
        {
            const uint polynomial =
                0xEDB88320;

            uint[] table =
                new uint[256];

            for (uint i = 0; i < 256; i++)
            {
                uint value = i;

                for (int j = 0; j < 8; j++)
                {
                    if ((value & 1) != 0)
                    {
                        value =
                            (value >> 1) ^
                            polynomial;
                    }
                    else
                    {
                        value >>= 1;
                    }
                }

                table[i] = value;
            }

            return table;
        }
    }
}

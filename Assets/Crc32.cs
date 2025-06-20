using System;

public static class Crc32
{
    private static readonly uint[] Table = new uint[256];

    static Crc32()
    {
        for (uint i = 0; i < 256; i++)
        {
            uint crc = i;
            for (int j = 0; j < 8; j++)
                crc = (crc >> 1) ^ (0xEDB88320u & (uint)(-(crc & 1)));
            Table[i] = crc;
        }
    }

    public static uint Compute(byte[] data)
    {
        uint crc = 0xFFFFFFFFu;
        foreach (byte b in data)
            crc = (crc >> 8) ^ Table[(crc ^ b) & 0xFF];
        return crc ^ 0xFFFFFFFFu;
    }
}
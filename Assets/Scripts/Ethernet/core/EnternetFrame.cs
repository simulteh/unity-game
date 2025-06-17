using System; // Äëÿ DateTime
using System.IO; // Äëÿ MemoryStream
using System.Linq; // Äëÿ LINQ
using UnityEngine;

public class EthernetFrame
{
    public readonly byte[] Preamble = { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
    public MACAddress DestinationAddress { get; }
    public byte[] Payload { get; }

    public EthernetFrame(MACAddress dest, MACAddress src, ushort type, byte[] payload)
    {
        DestinationAddress = dest ?? throw new ArgumentNullException(nameof(dest));
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
    }

    public byte[] Serialize()
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms))
        {
            writer.Write(Preamble);
            writer.Write(DestinationAddress.ToByteArray());
            writer.Write(Payload);
            return ms.ToArray();
        }
    }
}
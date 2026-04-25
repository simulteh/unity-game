using System;

[Serializable]
public class Packet
{
    public IPAddress SourceIP { get; set; }
    public IPAddress DestinationIP { get; set; }
    public MACAddress SourceMAC { get; set; }
    public MACAddress DestinationMAC { get; set; }
    public int TTL { get; set; } = 64;
    public string Protocol { get; set; }
    public string Data { get; set; }

    public override string ToString()
    {
        return $"Packet [{Protocol}] from {SourceIP}:{SourceMAC} to {DestinationIP}:{DestinationMAC}, TTL: {TTL}, Data: {Data}";
    }
}
using System;

[Serializable]
public class NetworkFrame
{
    public string SourceMac;
    public string DestinationMac;
    public int VlanId;
    public string Payload;
    public DateTime Timestamp;

    public NetworkFrame(string srcMac, string dstMac, int vlanId, string payload)
    {
        SourceMac = srcMac;
        DestinationMac = dstMac;
        VlanId = vlanId;
        Payload = payload;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"Frame: {SourceMac} → {DestinationMac} | VLAN {VlanId} | \"{Payload}\"";
    }
}
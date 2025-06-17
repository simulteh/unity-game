// FirewallSystem.cs
public class FirewallSystem : NetworkBehaviour
{
    public List<string> allowedIPs = new();
    public bool blockAllIncoming = false;

    public bool CheckPacket(AdvancedNetworkPacket.PacketData packet)
    {
        if (blockAllIncoming && packet.DestinationIP == ipAddress)
            return false;
            
        return allowedIPs.Contains(packet.SourceIP);
    }
}
// VirusSystem.cs
public class VirusSystem : NetworkBehaviour
{
    public enum VirusType
    {
        TROJAN,
        WORM,
        RANSOMWARE
    }

    public void InjectVirus(string targetIP, VirusType type)
    {
        var packetData = new AdvancedNetworkPacket.PacketData
        {
            SourceIP = "MALICIOUS_SOURCE",
            DestinationIP = targetIP,
            Payload = type.ToString(),
            TTL = 255,
            IsEncrypted = true
        };
        
        // Логика отправки вируса
    }
}
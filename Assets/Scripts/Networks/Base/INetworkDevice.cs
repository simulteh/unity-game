public interface INetworkDevice
{
    IPAddress IPAddress { get; }
    MACAddress MACAddress { get; }
    string Name { get; }
    void ReceivePacket(Packet packet);
}

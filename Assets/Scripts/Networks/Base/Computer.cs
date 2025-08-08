using UnityEngine;
using System.Collections.Generic;

public class Computer : INetworkDevice
{
    public IPAddress IPAddress { get; private set; }
    public MACAddress MACAddress { get; private set; }
    public string Name { get; private set; }
    private ARP arp;
    private List<Computer> networkDevices;

    public Computer(string name, string ip, string mac, List<Computer> networkDevices)
    {
        Name = name;
        IPAddress = new IPAddress(ip);
        MACAddress = new MACAddress(mac);
        this.networkDevices = networkDevices;
        this.arp = new ARP(this);
    }

    public void SendPacket(IPAddress destinationIP, string protocol, string data)
    {
        var destMAC = arp.Resolve(destinationIP);
        if (destMAC == null)
        {
            Debug.Log($"Failed to resolve MAC for {destinationIP}");
            return;
        }

        var packet = new Packet
        {
            SourceIP = IPAddress,
            DestinationIP = destinationIP,
            SourceMAC = MACAddress,
            DestinationMAC = destMAC,
            Protocol = protocol,
            Data = data
        };

        Ethernet.SendFrame(packet, this, networkDevices);
    }

    public void ReceivePacket(Packet packet)
    {
        Debug.Log($"{Name} received packet: {packet.Data}");
    }
}

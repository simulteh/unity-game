using UnityEngine;
using System.Collections.Generic;

public class Ethernet
{
    
    public static void SendFrame(Packet packet, INetworkDevice sender, IEnumerable<INetworkDevice> networkDevices)
    {
        Debug.Log($"Ethernet frame sent: {packet}");

        foreach (var device in networkDevices)
        {
            if (device.IPAddress.Address == packet.DestinationIP.Address)
            {
                device.ReceivePacket(packet);
                return;
            }
        }

        Debug.Log($"Device with IP {packet.DestinationIP} not found in the network");
    }
}
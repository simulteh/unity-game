using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Router : INetworkDevice
{
    public IPAddress IPAddress { get; private set; }
    public MACAddress MACAddress { get; private set; }
    public string Name { get; private set; }
    private RoutingTable routingTable = new RoutingTable();
    private List<Computer> networkDevices;

    public Router(string name, string ip, string mac, List<Computer> networkDevices)
    {
        Name = name;
        IPAddress = new IPAddress(ip);
        MACAddress = new MACAddress(mac);
        this.networkDevices = networkDevices;
    }

    public void AddRoute(IPAddress network, string netmask, IPAddress nextHop)
    {
        routingTable.AddRoute(network, netmask, nextHop);
    }

    public void ReceivePacket(Packet packet)
    {
        if (packet.DestinationIP.Equals(IPAddress))
        {
            Debug.Log($"{Name} received packet for itself: {packet.Data}");
            return;
        }

        var nextHop = routingTable.GetNextHop(packet.DestinationIP);
        if (nextHop != null)
        {
            Debug.Log($"{Name} routing packet to {nextHop}");
            packet.TTL--;
            if (packet.TTL > 0)
            {
                var destMAC = new MACAddress($"00:00:00:00:{nextHop.Address.Split('.')[2]}:{nextHop.Address.Split('.')[3]}");

                var newPacket = new Packet
                {
                    SourceIP = packet.SourceIP,
                    DestinationIP = packet.DestinationIP,
                    SourceMAC = MACAddress,
                    DestinationMAC = destMAC,
                    Protocol = packet.Protocol,
                    Data = packet.Data,
                    TTL = packet.TTL
                };

                //Ethernet.SendFrame(newPacket, this, networkDevices);
            }
        }
    }
}


public class RoutingTable
{
    private List<RouteEntry> routes = new List<RouteEntry>();

    public void AddRoute(IPAddress network, string netmask, IPAddress nextHop)
    {
        routes.Add(new RouteEntry(network, netmask, nextHop));
    }

    public IPAddress GetNextHop(IPAddress destination)
    {
        foreach (var route in routes)
        {
            if (route.Matches(destination))
            {
                return route.NextHop;
            }
        }
        return null;
    }

    private class RouteEntry
    {
        public IPAddress Network { get; }
        public string Netmask { get; }
        public IPAddress NextHop { get; }

        public RouteEntry(IPAddress network, string netmask, IPAddress nextHop)
        {
            Network = network;
            Netmask = netmask;
            NextHop = nextHop;
        }

        public bool Matches(IPAddress address)
        {
            // Simplified network matching
            return address.Address.StartsWith(Network.Address.Substring(0, Network.Address.LastIndexOf('.')));
        }
    }
}
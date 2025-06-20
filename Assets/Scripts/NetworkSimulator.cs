using System;
using System.Collections.Generic;

public class NetworkSimulator
{
    public NAT nat { get; private set; }
    public Dictionary<string, string> internal_devices { get; private set; }

    public NetworkSimulator(string external_ip, Tuple<int, int> port_range)
    {
        nat = new NAT(external_ip, port_range);
        internal_devices = new Dictionary<string, string>();
    }

    public void AddDevice(string internal_ip)
    {
        if (!internal_devices.ContainsKey(internal_ip))
        {
            internal_devices.Add(internal_ip, "Active");
            nat.AddInternalAddress(internal_ip);
        }
    }

    public void SimulateOutboundPacket(string internal_ip, int internal_port, string protocol)
    {
        var result = nat.TranslateOutbound(internal_ip, internal_port, protocol);
        if (result != null)
        {
            Console.WriteLine($"Outbound: {internal_ip}:{internal_port} -> {result.Item1}:{result.Item2} ({protocol})");
        }
    }

    public void SimulateInboundPacket(int external_port, string protocol)
    {
        var result = nat.TranslateInbound(external_port, protocol);
        if (result != null)
        {
            Console.WriteLine($"Inbound: {nat.external_ip}:{external_port} -> {result.Item1}:{result.Item2} ({protocol})");
        }
    }

    public void DisplayTranslationTable()
    {
        Console.WriteLine("Translation Table:");
        foreach (var entry in nat.translation_table)
        {
            if (entry.Value != null)
            {
                Console.WriteLine($"{entry.Key.Item1}:{entry.Key.Item2} <-> {nat.external_ip}:{entry.Value.Item1} ({entry.Value.Item2})");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class NAT
{
    public string external_ip { get; private set; }
    public Tuple<int, int> port_range { get; private set; }
    public Dictionary<Tuple<string, int>, Tuple<int, string>> translation_table { get; private set; }
    public HashSet<int> used_ports { get; private set; }

    public NAT(string external_ip, Tuple<int, int> port_range)
    {
        this.external_ip = external_ip;
        this.port_range = port_range;
        this.translation_table = new Dictionary<Tuple<string, int>, Tuple<int, string>>();
        this.used_ports = new HashSet<int>();
    }

    public void AddInternalAddress(string internal_ip)
    {
        var baseKey = new Tuple<string, int>(internal_ip, 0);
        if (!translation_table.ContainsKey(baseKey))
        {
            translation_table.Add(baseKey, null);
        }
    }

    public Tuple<string, int> TranslateOutbound(string internal_ip, int internal_port, string protocol)
    {
        var baseKey = new Tuple<string, int>(internal_ip, 0);
        if (!translation_table.ContainsKey(baseKey))
        {
            Debug.LogError($"Error: Internal address {internal_ip} not registered");
            return null;
        }

        var key = new Tuple<string, int>(internal_ip, internal_port);
        int external_port = GeneratePort();
        if (external_port == -1)
        {
            Debug.LogError("Error: No available ports");
            return null;
        }

        translation_table[key] = new Tuple<int, string>(external_port, protocol);
        used_ports.Add(external_port);

        Debug.Log($"Translated: {internal_ip}:{internal_port} -> {external_ip}:{external_port} ({protocol})");
        return new Tuple<string, int>(external_ip, external_port);
    }

    public Tuple<string, int> TranslateInbound(int external_port, string protocol)
    {
        foreach (var entry in translation_table)
        {
            if (entry.Value != null &&
                entry.Value.Item1 == external_port &&
                entry.Value.Item2 == protocol)
            {
                Debug.Log($"Reverse translated: {external_ip}:{external_port} -> {entry.Key.Item1}:{entry.Key.Item2}");
                return entry.Key;
            }
        }
        Debug.LogError($"Error: No mapping found for {external_ip}:{external_port} ({protocol})");
        return null;
    }

    public void ReleasePort(int external_port)
    {
        if (used_ports.Contains(external_port))
        {
            used_ports.Remove(external_port);
            Debug.Log($"Released port: {external_port}");
        }
    }

    private int GeneratePort()
    {
        for (int port = port_range.Item1; port <= port_range.Item2; port++)
        {
            if (!used_ports.Contains(port))
            {
                return port;
            }
        }
        return -1;
    }
}
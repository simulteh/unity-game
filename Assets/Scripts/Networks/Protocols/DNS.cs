using UnityEngine;
using System.Collections.Generic;

public class DNS
{
    private Dictionary<string, IPAddress> dnsRecords = new Dictionary<string, IPAddress>();

    public void AddRecord(string domain, IPAddress ip)
    {
        dnsRecords[domain] = ip;
    }

    public IPAddress Resolve(string domain)
    {
        if (dnsRecords.TryGetValue(domain, out var ip))
        {
            Debug.Log($"DNS resolved {domain} to {ip}");
            return ip;
        }
        Debug.Log($"DNS resolution failed for {domain}");
        return null;
    }
}
using UnityEngine;
using System.Collections.Generic;

public class NAT
{
    private IPAddress publicIP;
    private Dictionary<IPAddress, int> translationTable = new Dictionary<IPAddress, int>();
    private int nextPort = 1024;

    public NAT(string publicIP)
    {
        this.publicIP = new IPAddress(publicIP);
    }

    public IPAddress TranslateOutbound(IPAddress privateIP)
    {
        if (!translationTable.ContainsKey(privateIP))
        {
            translationTable[privateIP] = nextPort++;
        }
        Debug.Log($"NAT translating {privateIP} to {publicIP}:{translationTable[privateIP]}");
        return publicIP;
    }

    public IPAddress TranslateInbound(int port)
    {
        foreach (var entry in translationTable)
        {
            if (entry.Value == port)
            {
                Debug.Log($"NAT translating {publicIP}:{port} back to {entry.Key}");
                return entry.Key;
            }
        }
        return null;
    }
}

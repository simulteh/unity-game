using System.Collections.Generic;
using UnityEngine;

public class ARP
{
    private Dictionary<IPAddress, MACAddress> arpTable = new Dictionary<IPAddress, MACAddress>();
    private Computer owner;

    public ARP(Computer owner)
    {
        this.owner = owner;
    }

    public MACAddress Resolve(IPAddress ip)
    {
        if (arpTable.TryGetValue(ip, out var mac))
        {
            Debug.Log($"ARP resolved {ip} to {mac}");
            return mac;
        }

        Debug.Log($"ARP request for {ip}");
        // В реальной сети здесь был бы широковещательный запрос
        // В нашем случае просто возвращаем MAC на основе IP (упрощенно)
        return new MACAddress($"00:00:00:00:{ip.Address.Split('.')[2]}:{ip.Address.Split('.')[3]}");
    }

    public void AddEntry(IPAddress ip, MACAddress mac)
    {
        arpTable[ip] = mac;
    }
}

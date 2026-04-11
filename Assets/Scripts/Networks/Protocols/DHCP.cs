using UnityEngine;

public class DHCP
{
    private IPAddress[] addressPool;
    private int currentIndex = 0;

    public DHCP(string poolStart, string poolEnd)
    {
        var start = IPAddressToInt(poolStart);
        var end = IPAddressToInt(poolEnd);
        addressPool = new IPAddress[end - start + 1];

        for (int i = 0; i < addressPool.Length; i++)
        {
            addressPool[i] = new IPAddress(IntToIPAddress(start + i));
        }
    }

    public IPAddress AssignIP()
    {
        if (currentIndex >= addressPool.Length)
        {
            Debug.Log("DHCP pool exhausted");
            return null;
        }

        var ip = addressPool[currentIndex++];
        Debug.Log($"DHCP assigned IP: {ip}");
        return ip;
    }

    private int IPAddressToInt(string ip)
    {
        var parts = ip.Split('.');
        return (int.Parse(parts[0]) << 24) | (int.Parse(parts[1]) << 16) |
               (int.Parse(parts[2]) << 8) | int.Parse(parts[3]);
    }

    private string IntToIPAddress(int ip)
    {
        return $"{(ip >> 24) & 0xFF}.{(ip >> 16) & 0xFF}.{(ip >> 8) & 0xFF}.{ip & 0xFF}";
    }
}

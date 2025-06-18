using UnityEngine;

public class Interface
{
    public string InterfaceId { get; set; } // Например, "eth0", "eth1"
    public string IpAddress { get; set; }
    public string SubnetMask { get; set; }
    public bool IsUp { get; set; } = true; // Состояние интерфейса (включен/выключен)

    public Interface(string interfaceId, string ipAddress, string subnetMask)
    {
        InterfaceId = interfaceId;
        IpAddress = ipAddress;
        SubnetMask = subnetMask;
    }

    public override string ToString()
    {
        return $"Interface(Id: {InterfaceId}, IP: {IpAddress}/{SubnetMask}, Status: {(IsUp ? "Up" : "Down")})";
    }
}

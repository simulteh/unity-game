using System;

[Serializable]
public class IPAddress
{
    public string Address { get; private set; }

    public IPAddress(string address)
    {
        if (!ValidateIP(address))
            throw new ArgumentException("Invalid IP address format");
        Address = address;
    }

    private bool ValidateIP(string address)
    {
        return System.Net.IPAddress.TryParse(address, out _);
    }

    public override string ToString() => Address;
}
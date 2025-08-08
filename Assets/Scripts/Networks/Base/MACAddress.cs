using System; // Äîáàâëåíî äëÿ Convert

[Serializable]
public class MACAddress
{
    public string Address { get; private set; }

    public MACAddress(string address)
    {
        if (!ValidateMAC(address))
            throw new ArgumentException("Invalid MAC address format");
        Address = address;
    }

    private bool ValidateMAC(string address)
    {
        // Простая валидация MAC-адреса (формат XX:XX:XX:XX:XX:XX)
        var parts = address.Split(':');
        if (parts.Length != 6) return false;

        foreach (var part in parts)
        {
            if (part.Length != 2) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(part, "^[0-9A-Fa-f]{2}$"))
                return false;
        }

        return true;
    }

    public override string ToString() => Address;
}
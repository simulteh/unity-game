using System.Text.RegularExpressions;

[System.Serializable]
public class IPGameData
{
    public string IP { get; private set; }

    public IPGameData(string ip = null)
    {
        if (ip != null && IsValidIP(ip))
        {
            IP = ip;
        }
        else
        {
            // Generate random valid IP
            IP = $"{Random.Range(0, 256)}.{Random.Range(0, 256)}.{Random.Range(0, 256)}.{Random.Range(0, 256)}";
            
            // 25% chance to generate a private IP
            if (Random.value < 0.25f)
            {
                int privateType = Random.Range(0, 3);
                switch (privateType)
                {
                    case 0: // 10.0.0.0/8
                        IP = $"10.{Random.Range(0, 256)}.{Random.Range(0, 256)}.{Random.Range(0, 256)}";
                        break;
                    case 1: // 172.16.0.0/12
                        IP = $"172.{Random.Range(16, 32)}.{Random.Range(0, 256)}.{Random.Range(0, 256)}";
                        break;
                    case 2: // 192.168.0.0/16
                        IP = $"192.168.{Random.Range(0, 256)}.{Random.Range(0, 256)}";
                        break;
                }
            }
        }
    }

    public static bool IsValidIP(string ip)
    {
        return Regex.IsMatch(ip, 
            @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    }

    public string GetClass()
    {
        int firstOctet = int.Parse(IP.Split('.')[0]);
        
        if (firstOctet < 128) return "A";
        if (firstOctet < 192) return "B";
        if (firstOctet < 224) return "C";
        if (firstOctet < 240) return "D";
        return "E";
    }

    public bool IsPrivate()
    {
        string[] octets = IP.Split('.');
        int first = int.Parse(octets[0]);
        int second = int.Parse(octets[1]);
        
        // 10.0.0.0/8
        if (first == 10) return true;
        
        // 172.16.0.0/12
        if (first == 172 && second >= 16 && second <= 31) return true;
        
        // 192.168.0.0/16
        if (first == 192 && second == 168) return true;
        
        // 127.0.0.0/8 (loopback)
        if (first == 127) return true;
        
        return false;
    }

    public string GetBinary()
    {
        string[] octets = IP.Split('.');
        string binary = "";
        
        for (int i = 0; i < 4; i++)
        {
            binary += Convert.ToString(int.Parse(octets[i]), 2).PadLeft(8, '0');
            if (i < 3) binary += ".";
        }
        
        return binary;
    }
}
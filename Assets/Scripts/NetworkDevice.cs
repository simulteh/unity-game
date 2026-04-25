using UnityEngine;

public class NetworkDevice : MonoBehaviour
{
        public string Name { get; private set; }
        public string IP { get; set; }
        public string Gateway { get; set; }
        public string DNS { get; set; }
        public bool UseDHCP { get; set; }

        public NetworkDevice(string name, string ip, bool useDHCP = false)
        {
            Name = name;
            IP = ip;
            UseDHCP = useDHCP;
            Gateway = "192.168.1.1";
            DNS = "8.8.8.8";
        }

        public bool IsConfiguredProperly()
        {
            if (UseDHCP)
                return true; // Допустим, DHCP работает правильно

            // Примитивные проверки — расширяемые
            return IsValidIP(IP) && IsValidIP(Gateway) && IsValidIP(DNS);
        }

        private bool IsValidIP(string ip)
        {
            System.Net.IPAddress address;
            return System.Net.IPAddress.TryParse(ip, out address);
        }
    }


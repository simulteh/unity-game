using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkDevice : MonoBehaviour
{
    [SerializeField] private string macAddress;
    [SerializeField] private string ipAddress;
    [SerializeField] private string subnetMask;
    [SerializeField] private string gateway;
    [SerializeField] private Dictionary<string, string> arpTable;
    [SerializeField] private EthernetPort ethernetPort;

    public string MacAddress
    {
        get => macAddress;
        protected set => macAddress = value;
    }

    public string IpAddress
    {
        get => ipAddress;
        protected set => ipAddress = value;
    }

    public string SubnetMask
    {
        get => subnetMask;
        protected set => subnetMask = value;
    }

    public string Gateway
    {
        get => gateway;
        protected set => gateway = value;
    }

    public Dictionary<string, string> ArpTable
    {
        get => arpTable;
        protected set => arpTable = value;
    }

    public EthernetPort EthernetPort
    {
        get => ethernetPort;
        protected set => ethernetPort = value;
    }

    protected void Start()
    {
        GenerateMacAddress();
        ArpTable = new Dictionary<string, string>();
        EthernetPort = new EthernetPort(this);
    }

    protected void GenerateMacAddress()
    {
        byte[] macAddressBytes = new byte[6];
        System.Random random = new System.Random();
        random.NextBytes(macAddressBytes);

        // Устанавливаем первый байт, чтобы MAC-адрес был локально администрируемым
        macAddressBytes[0] = (byte)(macAddressBytes[0] | 0x02);

        MacAddress = System.BitConverter.ToString(macAddressBytes).Replace("-", ":");
        Debug.Log("Generated MAC Address: " + MacAddress);
    }

    public void AssignIpAddress(string ipAddress, string subnetMask, string gateway)
    {
        IpAddress = ipAddress;
        SubnetMask = subnetMask;
        Gateway = gateway;

        Debug.Log("Assigned IP Address: " + IpAddress);
        Debug.Log("Assigned Subnet Mask: " + SubnetMask);
        if (Gateway != null)
        {
            Debug.Log("Assigned Gateway: " + Gateway);
        }
    }

    public void AddArpEntry(string ipAddress, string macAddress)
    {
        if (!ArpTable.ContainsKey(ipAddress))
        {
            ArpTable[ipAddress] = macAddress;
            Debug.Log("Added ARP entry: " + ipAddress + " -> " + macAddress);
        }
    }

    public string GetMacAddressFromArpTable(string ipAddress)
    {
        if (ArpTable.TryGetValue(ipAddress, out string macAddress))
        {
            return macAddress;
        }
        return null;
    }
}
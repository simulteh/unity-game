using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthernetPort
{
    public Computer ParentComputer { get; private set; }
    public EthernetCable ConnectedCable { get; private set; }

    public EthernetPort(Computer parentComputer)
    {
        ParentComputer = parentComputer;
    }

    public void ConnectCable(EthernetCable cable)
    {
        if (ConnectedCable != null)
        {

            return;
        }

        ConnectedCable = cable;
        cable.ConnectPort(this);
        Debug.Log("Соединен с " + ParentComputer.IpAddress);
    }

    public void DisconnectCable()
    {
        if (ConnectedCable != null)
        {
            ConnectedCable.DisconnectPort(this);
            ConnectedCable = null;
            Debug.Log("Отсоединен от " + ParentComputer.IpAddress);
        }
    }
}

public class EthernetCable
{
    public EthernetPort Port1 { get; private set; }
    public EthernetPort Port2 { get; private set; }

    public void ConnectPort(EthernetPort port)
    {
        if (Port1 == null)
        {
            Port1 = port;
        }
        else if (Port2 == null)
        {
            Port2 = port;
            Debug.Log("Соединение между " + Port1.ParentComputer.IpAddress + " и " + Port2.ParentComputer.IpAddress);
        }
        else
        {
            Debug.LogError("Уже соединен.");
        }
    }

    public void DisconnectPort(EthernetPort port)
    {
        if (Port1 == port)
        {
            Port1 = null;
        }
        else if (Port2 == port)
        {
            Port2 = null;
        }

        if (Port1 == null && Port2 == null)
        {
            Debug.Log(".");
        }
    }
}

public class Computer : MonoBehaviour
{
    public string MacAddress { get; private set; }
    public string IpAddress { get; private set; }
    public string SubnetMask { get; private set; }
    public string Gateway { get; private set; }
    public Dictionary<string, string> ArpTable { get; private set; }
    public EthernetPort EthernetPort { get; private set; }

    private void Start()
    {
        GenerateMacAddress();
        AssignIpAddress("192.168.1.100", "255.255.255.0", "192.168.1.1");
        ArpTable = new Dictionary<string, string>();
        EthernetPort = new EthernetPort(this);
    }

    private void GenerateMacAddress()
    {
        byte[] macAddressBytes = new byte[6];
        System.Random random = new System.Random();
        random.NextBytes(macAddressBytes);

        macAddressBytes[0] = (byte)(macAddressBytes[0] | 0x02);

        MacAddress = System.BitConverter.ToString(macAddressBytes).Replace("-", ":");
        Debug.Log("MAC-Адрес: " + MacAddress);
    }

    public void AssignIpAddress(string ipAddress, string subnetMask, string gateway)
    {
        IpAddress = ipAddress;
        SubnetMask = subnetMask;
        Gateway = gateway;

        Debug.Log("IP: " + IpAddress);
        Debug.Log("Subnet Mask: " + SubnetMask);
        Debug.Log("Gateway: " + Gateway);
    }

    public void SendData(string data, Computer destination)
    {
        if (destination != null)
        {
            Debug.Log("Оотправка данных от " + IpAddress + " до " + destination.IpAddress + ": " + data);
            destination.ReceiveData(data, this);
        }
        else
        {
            Debug.LogError("Null.");
        }
    }

    public void ReceiveData(string data, Computer source)
    {
        Debug.Log("Получены данные от " + source.IpAddress + ": " + data);
    }

    public void AddArpEntry(string ipAddress, string macAddress)
    {
        if (!ArpTable.ContainsKey(ipAddress))
        {
            ArpTable[ipAddress] = macAddress;
            Debug.Log("APR добавлен: " + ipAddress + " -> " + macAddress);
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

    public void Ping(Computer destination)
    {
        if (destination != null)
        {
            Debug.Log("Pinging " + destination.IpAddress);
            destination.ReceivePing(this);
        }
        else
        {
            Debug.LogError("Null.");
        }
    }

    public void ReceivePing(Computer source)
    {
        Debug.Log("Ping получен " + source.IpAddress);
    }
}
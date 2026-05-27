using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SwitchPort
{
    public int PortNumber;
    public NetworkDevice ConnectedDevice;
    public PortMode Mode;
    public int AccessVlan;
    public List<int> AllowedVlans;
    public bool IsTrunk;
}

public enum PortMode
{
    Access,
    Trunk
}

[System.Serializable]
public class MacTableEntry
{
    public NetworkDevice Device;
    public int VlanId;
    public int PortNumber;
}

public class Switch : MonoBehaviour
{
    [Header("Конфигурация портов")]
    public List<SwitchPort> ports = new List<SwitchPort>();

    [Header("Таблица MAC-адресов")]
    public Dictionary<string, MacTableEntry> macTable = new Dictionary<string, MacTableEntry>();

    [Header("Подключённые устройства")]
    public List<NetworkDevice> connectedDevices = new List<NetworkDevice>();

    public void RegisterDevice(NetworkDevice device)
    {
        if (!connectedDevices.Contains(device))
        {
            connectedDevices.Add(device);
            Debug.Log($"[Switch] Зарегистрировано устройство: {device.Name} (VLAN {device.VlanId})");
        }
    }

    public void RegisterDevice(int portNumber, NetworkDevice device, bool isTrunk = false)
    {
        if (!connectedDevices.Contains(device))
            connectedDevices.Add(device);

        var existingPort = ports.Find(p => p.PortNumber == portNumber);
        if (existingPort != null)
        {
            existingPort.ConnectedDevice = device;
            existingPort.IsTrunk = isTrunk;
            existingPort.Mode = isTrunk ? PortMode.Trunk : PortMode.Access;
            existingPort.AccessVlan = device.VlanId;
        }
        else
        {
            var newPort = new SwitchPort
            {
                PortNumber = portNumber,
                ConnectedDevice = device,
                Mode = isTrunk ? PortMode.Trunk : PortMode.Access,
                AccessVlan = device.VlanId,
                IsTrunk = isTrunk,
                AllowedVlans = isTrunk ? new List<int> { device.VlanId } : null
            };
            ports.Add(newPort);
        }

        Debug.Log($"[Switch] Устройство {device.Name} подключено к порту {portNumber} ({(isTrunk ? "Trunk" : "Access")}, VLAN {device.VlanId})");
    }

    public int GetVlanForDevice(NetworkDevice device)
    {
        var port = ports.Find(p => p.ConnectedDevice == device);
        if (port != null)
        {
            if (port.Mode == PortMode.Access)
                return port.AccessVlan;
        }
        return device.VlanId;
    }

    private int GetPortNumberForDevice(NetworkDevice device)
    {
        var port = ports.Find(p => p.ConnectedDevice == device);
        return port?.PortNumber ?? -1;
    }

    public void ReceiveFrame(NetworkFrame frame, NetworkDevice sourceDevice)
    {
        Debug.Log($"[Switch] Получен кадр от {sourceDevice.Name} (MAC: {frame.SourceMac}, VLAN: {frame.VlanId})");

        int sourceVlan = GetVlanForDevice(sourceDevice);

        if (sourceVlan != frame.VlanId)
        {
            Debug.LogWarning($"[Switch] Несовпадение VLAN! Устройство {sourceVlan} != Кадр {frame.VlanId}");
            return;
        }

        if (!macTable.ContainsKey(frame.SourceMac))
        {
            macTable[frame.SourceMac] = new MacTableEntry
            {
                Device = sourceDevice,
                VlanId = sourceVlan,
                PortNumber = GetPortNumberForDevice(sourceDevice)
            };
            Debug.Log($"[Switch] MAC {frame.SourceMac} изучен на VLAN {sourceVlan}");
        }

        if (macTable.TryGetValue(frame.DestinationMac, out var destination))
        {
            if (destination.VlanId == sourceVlan)
            {
                Debug.Log($"[Switch] Кадр отправлен напрямую → {destination.Device.Name}");
                destination.Device.ReceiveFrame(frame);
            }
            else
            {
                Debug.Log($"[Switch] Получатель {frame.DestinationMac} в другом VLAN {destination.VlanId}, кадр заблокирован");
            }
        }
        else
        {
            Debug.Log($"[Switch] MAC {frame.DestinationMac} неизвестен, флуд в VLAN {sourceVlan}");
            FloodWithinVlan(frame, sourceVlan, sourceDevice);
        }
    }

    private void FloodWithinVlan(NetworkFrame frame, int vlanId, NetworkDevice sender)
    {
        foreach (var device in connectedDevices)
        {
            if (device == sender) continue;

            if (device.VlanId == vlanId)
            {
                Debug.Log($"[Switch] Флуд → {device.Name}");
                device.ReceiveFrame(frame);
            }
        }
    }
}

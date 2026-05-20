using System.Collections.Generic;
using UnityEngine;

// ========== ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ ==========

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

// ========== ОСНОВНОЙ КЛАСС КОММУТАТОРА ==========

public class Switch : MonoBehaviour
{
    [Header("Port Configuration")]
    public List<SwitchPort> ports = new List<SwitchPort>();

    [Header("MAC Address Table")]
    public Dictionary<string, MacTableEntry> macTable = new Dictionary<string, MacTableEntry>();

    [Header("Connected Devices")]
    public List<NetworkDevice> connectedDevices = new List<NetworkDevice>();

    // Регистрация устройства (простая)
    public void RegisterDevice(NetworkDevice device)
    {
        if (!connectedDevices.Contains(device))
        {
            connectedDevices.Add(device);
            Debug.Log($"[Switch] Registered device: {device.Name} with VLAN {device.VlanId}");
        }
    }

    // Регистрация устройства с портом
    public void RegisterDevice(int portNumber, NetworkDevice device, bool isTrunk = false)
    {
        // Добавляем в список устройств
        if (!connectedDevices.Contains(device))
            connectedDevices.Add(device);

        // Создаём или обновляем порт
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

        Debug.Log($"[Switch] Device {device.Name} registered on port {portNumber} ({(isTrunk ? "Trunk" : "Access")}, VLAN {device.VlanId})");
    }

    // Получить VLAN для устройства
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

    // Получить номер порта для устройства
    private int GetPortNumberForDevice(NetworkDevice device)
    {
        var port = ports.Find(p => p.ConnectedDevice == device);
        return port?.PortNumber ?? -1;
    }

    // Основной метод приёма кадра
    public void ReceiveFrame(NetworkFrame frame, NetworkDevice sourceDevice)
    {
        Debug.Log($"[Switch] Received frame from {sourceDevice.Name} (MAC: {frame.SourceMac}, VLAN: {frame.VlanId})");

        // Узнаём VLAN источника
        int sourceVlan = GetVlanForDevice(sourceDevice);

        // Если VLAN не совпадает с VLAN кадра (для access портов)
        if (sourceVlan != frame.VlanId)
        {
            Debug.LogWarning($"[Switch] VLAN mismatch! Source device VLAN {sourceVlan} != Frame VLAN {frame.VlanId}");
            return;
        }

        // Обучаем MAC-таблицу
        if (!macTable.ContainsKey(frame.SourceMac))
        {
            macTable[frame.SourceMac] = new MacTableEntry
            {
                Device = sourceDevice,
                VlanId = sourceVlan,
                PortNumber = GetPortNumberForDevice(sourceDevice)
            };
            Debug.Log($"[Switch] Learned MAC {frame.SourceMac} on VLAN {sourceVlan}");
        }

        // Ищем получателя
        if (macTable.TryGetValue(frame.DestinationMac, out var destination))
        {
            // Проверяем, что в том же VLAN
            if (destination.VlanId == sourceVlan)
            {
                Debug.Log($"[Switch] Forwarding frame directly to {destination.Device.Name}");
                destination.Device.ReceiveFrame(frame);
            }
            else
            {
                Debug.Log($"[Switch] Destination {frame.DestinationMac} is in different VLAN {destination.VlanId}, blocking frame");
            }
        }
        else
        {
            // Flood внутри VLAN
            Debug.Log($"[Switch] MAC {frame.DestinationMac} unknown, flooding within VLAN {sourceVlan}");
            FloodWithinVlan(frame, sourceVlan, sourceDevice);
        }
    }

    // Рассылка всем устройствам в VLAN (кроме отправителя)
    private void FloodWithinVlan(NetworkFrame frame, int vlanId, NetworkDevice sender)
    {
        foreach (var device in connectedDevices)
        {
            // Не отправляем обратно отправителю
            if (device == sender) continue;

            // Проверяем, что устройство в том же VLAN
            if (device.VlanId == vlanId)
            {
                Debug.Log($"[Switch] Flooding frame to {device.Name}");
                device.ReceiveFrame(frame);
            }
        }
    }
}
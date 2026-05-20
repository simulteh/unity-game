using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkDevice : MonoBehaviour
{
    [Header("Network Identity")]
    [SerializeField] private string deviceName;
    [SerializeField] private string ipAddress;
    [SerializeField] private string macAddress;
    [SerializeField] private int vlanId = 1;

    [Header("Configuration")]
    [SerializeField] private bool isProperlyConfigured = true;

    [Header("Connections")]
    [SerializeField] private List<NetworkConnection> connections = new List<NetworkConnection>();

    // ========== —¬Œ…—“¬¿ ==========
    public string Name
    {
        get => string.IsNullOrEmpty(deviceName) ? gameObject.name : deviceName;
        set => deviceName = value;
    }

    public string IP
    {
        get => ipAddress;
        set => ipAddress = value;
    }

    public string MacAddress
    {
        get => macAddress;
        set => macAddress = value;
    }

    public int VlanId
    {
        get => vlanId;
        set => vlanId = value;
    }

    public List<NetworkConnection> Connections => connections;

    // ========== Ã≈“Œƒ€ ƒÀﬂ UI ==========
    // ›ÚÓ Ã≈“Œƒ, ‡ ÌÂ Ò‚ÓÈÒÚ‚Ó ó Ú‡Í ÓÊË‰‡ÂÚ AdminTableUI
    public bool IsConfiguredProperly()
    {
        return isProperlyConfigured;
    }

    public void SetConfiguredProperly(bool value)
    {
        isProperlyConfigured = value;
    }

    // ========== —≈“≈¬€≈ Ã≈“Œƒ€ ==========
    public void AddConnection(NetworkConnection connection)
    {
        if (!connections.Contains(connection))
            connections.Add(connection);
    }

    public void RemoveConnection(NetworkConnection connection)
    {
        connections.Remove(connection);
    }

    public void SendFrame(NetworkFrame frame, Switch targetSwitch)
    {
        if (targetSwitch != null)
        {
            Debug.Log($"[{Name}] Sending frame from {frame.SourceMac} to {frame.DestinationMac} (VLAN {frame.VlanId})");
            targetSwitch.ReceiveFrame(frame, this);
        }
    }

    public void ReceiveFrame(NetworkFrame frame)
    {
        Debug.Log($"[{Name}] Received frame from {frame.SourceMac}, payload: \"{frame.Payload}\" (VLAN {frame.VlanId})");
    }
}
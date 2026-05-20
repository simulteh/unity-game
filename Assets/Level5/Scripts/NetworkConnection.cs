using System;
using UnityEngine;

[Serializable]
public class NetworkConnection
{
    [SerializeField] private NetworkDevice connectedDevice;
    [SerializeField] private int portNumber;
    [SerializeField] private bool isTrunk;
    [SerializeField] private int trunkAllowedVlanStart = 1;
    [SerializeField] private int trunkAllowedVlanEnd = 4094;

    public NetworkDevice ConnectedDevice => connectedDevice;
    public int PortNumber => portNumber;
    public bool IsTrunk => isTrunk;
    public int TrunkAllowedVlanStart => trunkAllowedVlanStart;
    public int TrunkAllowedVlanEnd => trunkAllowedVlanEnd;

    public NetworkConnection(NetworkDevice device, int port, bool trunk = false)
    {
        connectedDevice = device;
        portNumber = port;
        isTrunk = trunk;
    }

    public bool IsVlanAllowedOnTrunk(int vlanId)
    {
        if (!isTrunk) return false;
        return vlanId >= trunkAllowedVlanStart && vlanId <= trunkAllowedVlanEnd;
    }

    public override string ToString()
    {
        return $"[Port {portNumber}] → {connectedDevice?.Name} ({(isTrunk ? "Trunk" : "Access")}, VLAN: {(isTrunk ? $"{trunkAllowedVlanStart}-{trunkAllowedVlanEnd}" : "N/A")})";
    }
}
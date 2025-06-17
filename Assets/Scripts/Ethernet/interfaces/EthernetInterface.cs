using System;
using UnityEngine;

public class EthernetInterface : MonoBehaviour {
    public MACAddress Address { get; private set; }
    public float SpeedMbps = 100f;

    // ������������� ����������
    public void Initialize(MACAddress address) {
        Address = address;
    }

    // �������� �����
    public void SendFrame(MACAddress dest, ushort type, byte[] payload) {
        var frame = new EthernetFrame(dest, Address, type, payload);
        EthernetSimulator.Instance.TransmitFrame(frame, this);
    }

    internal void ReceiveFrame(EthernetFrame frame)
    {
        throw new NotImplementedException();
    }
}
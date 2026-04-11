using UnityEngine;

public class UDP
{
    public void SendDatagram(IPAddress source, IPAddress destination, string data)
    {
        Debug.Log($"UDP datagram sent from {source} to {destination}: {data}");
    }
}
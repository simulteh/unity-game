using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : NetworkDevice
{
    public void SendData(string data, Computer destination)
    {
        if (destination != null)
        {
            Debug.Log("Sending data from " + IpAddress + " to " + destination.IpAddress + ": " + data);
            destination.ReceiveData(data, this);
        }
        else
        {
            Debug.LogError("Destination computer is null.");
        }
    }

    public void ReceiveData(string data, Computer source)
    {
        Debug.Log("Received data from " + source.IpAddress + ": " + data);
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
            Debug.LogError("Destination computer is null.");
        }
    }

    public void ReceivePing(Computer source)
    {
        Debug.Log("Received ping from " + source.IpAddress);
    }
}
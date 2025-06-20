using System;
using UnityEngine;

public class TestNAT : MonoBehaviour
{
    void Start()
    {
        NAT nat = new NAT("203.0.113.1", new Tuple<int, int>(49152, 49155));


        nat.AddInternalAddress("192.168.1.1");
        nat.AddInternalAddress("192.168.1.2");

        var tcpTranslation = nat.TranslateOutbound("192.168.1.1", 8080, "TCP");
        var udpTranslation = nat.TranslateOutbound("192.168.1.2", 9090, "UDP");


        if (tcpTranslation != null)
            nat.TranslateInbound(tcpTranslation.Item2, "TCP");

        if (udpTranslation != null)
            nat.TranslateInbound(udpTranslation.Item2, "UDP");

        nat.TranslateOutbound("192.168.1.99", 1234, "TCP"); 
        nat.TranslateInbound(9999, "TCP");
    }
}
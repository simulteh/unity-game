using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IpConfig : MonoBehaviour
{
    public string ip;
    public string subnetMask;
    public string gateway;


    private void Start()
    {
        ip = "0.0.0.0";
        subnetMask = "255.255.255.255";
        gateway = "0.0.0.0";
    }

}

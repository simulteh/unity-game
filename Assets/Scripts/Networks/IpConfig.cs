using UnityEngine;


public class IpConfig : MonoBehaviour
{
    public string ip;
    public string subnetMask;
    public string gateway;


    private void Start()
    {
        if (string.IsNullOrEmpty(ip) || ip == "0.0.0.0")
            ip = "0.0.0.0";
        if (string.IsNullOrEmpty(subnetMask))
            subnetMask = "255.255.255.255";
        if (string.IsNullOrEmpty(gateway))
            gateway = "0.0.0.0";
    }

}

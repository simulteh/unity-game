using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsIpConfig : MonoBehaviour
{
    [SerializeField] MouseDetector mouseDetector;

    [SerializeField] TMP_InputField inputField_ip;
    [SerializeField] TMP_InputField inputField_subnetMask;
    [SerializeField] TMP_InputField inputField_gateway;

    private void Start()
    {
        inputField_ip.text = "   .   .   .   ";
        inputField_subnetMask.text = "   .   .   .   ";
        inputField_gateway.text = "   .   .   .   ";
    }

    public void SetIpConfig()
    {
        IpConfig target = mouseDetector.target.GetComponent<IpConfig>();

        target.ip = inputField_ip.text;
        target.subnetMask = inputField_subnetMask.text;
        target.gateway = inputField_gateway.text;
    }
}

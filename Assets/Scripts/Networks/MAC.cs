using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAC : MonoBehaviour
{
    public string MACAddres;

    private void Awake()
    {
        GenerateMAC();
    }

    private void GenerateMAC() {
        string macAddress = "6A:13:E2";
        for (int i = 3; i < 6; i++) {
            byte value = (byte)Random.Range(0, 256);
            macAddress += ':' + value.ToString("X");
        }
        MACAddres = macAddress;
    }
}

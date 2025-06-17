using UnityEngine;
using System.Linq; 

[RequireComponent(typeof(Light))]
public class NetworkDevice : MonoBehaviour
{
    [Header("Настройки сети")]
    public string macAddress;
    public string ipAddress;

    [Header("Визуализация")]
    public Light statusLight;

    private DHCPServer dhcpServer;

    void Start()
    {
        dhcpServer = FindFirstObjectByType<DHCPServer>();
        if (dhcpServer == null)
        {
            Debug.LogError("DHCP Server not found!");
            return;
        }

        if (string.IsNullOrEmpty(macAddress))
            macAddress = GenerateRandomMAC();

        ipAddress = dhcpServer.RequestIP(macAddress);

        // Проверка IP (теперь будет работать)
        if (ipAddress != null && ipAddress.Count(c => c == '.') != 3)
        {
            Debug.LogError($"Invalid IP format: {ipAddress}");
            ipAddress = null;
        }

        if (statusLight != null)
            statusLight.color = ipAddress != null ? Color.green : Color.red;

        Debug.Log($"{gameObject.name} connected. MAC: {macAddress}, IP: {ipAddress ?? "N/A"}");
    }

    string GenerateRandomMAC()
    {
        System.Random rand = new System.Random();
        byte[] macBytes = new byte[6];
        rand.NextBytes(macBytes);
        return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
            macBytes[0], macBytes[1], macBytes[2],
            macBytes[3], macBytes[4], macBytes[5]);
    }

    void OnDestroy()
    {
        if (dhcpServer != null && !string.IsNullOrEmpty(ipAddress))
            dhcpServer.ReleaseIP(ipAddress);
    }
}
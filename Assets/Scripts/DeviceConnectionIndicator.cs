using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

[RequireComponent(typeof(Light))]
public class DeviceConnectionIndicator : MonoBehaviour
{
    [Header("Light Settings")]
    public Light statusLight;
    public Color connectedColor = Color.green;
    public Color disconnectedColor = Color.red;
    [Tooltip("���������� ������� ��� �����������")]
    public int blinkCount = 3;
    [Tooltip("������������ ������� (�������)")]
    public float blinkDuration = 0.3f;

    private DHCPServer _dhcpServer;
    private string ipAddress;
    private string _assignedIP;

    
    private void OnIPReleased(string ip)
    {
        if (_assignedIP == ip)
        {
            _assignedIP = null;
            SetLight(false);
            Debug.Log($"Disconnected: {ip}");
        }
    }

    private void OnIPLeased(string ip)
    {
        _assignedIP = ip;
        SetLight(true);
        Debug.Log($"Connected: {ip}");
    }
    // ������ ������� ����� ���������� ����������� �����
    private IEnumerator BlinkThenConnect()
    {
        bool currentState = statusLight.enabled;
        Color originalColor = statusLight.color;
        float originalIntensity = statusLight.intensity;

        // �������
        for (int i = 0; i < blinkCount; i++)
        {
            statusLight.color = connectedColor;
            statusLight.intensity = 5f; // ���� ��� �������
            yield return new WaitForSeconds(blinkDuration);

            statusLight.enabled = false;
            yield return new WaitForSeconds(blinkDuration);

            statusLight.enabled = true;
        }
        // ��������� ���������� ���������
        SetLight(true);
    }
        void Start()
    {
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        if (statusLight == null)
            statusLight = GetComponent<Light>();

        _dhcpServer = FindFirstObjectByType<DHCPServer>();
        if (_dhcpServer == null)
        {
            Debug.LogError("DHCP Server �� ������!");
            SetLight(false);
            return;
        }

        // 2. ������������� �� ��� �������
        _dhcpServer.OnIPLeased.AddListener(OnIPLeased);
        _dhcpServer.OnIPReleased.AddListener(OnIPReleased);

        ipAddress = _dhcpServer.RequestIP(GenerateMAC());
        SetLight(!string.IsNullOrEmpty(ipAddress));
    }

    void OnDestroy()
    {
        if (_dhcpServer != null)
        {
            _dhcpServer.OnIPLeased.RemoveListener(OnIPLeased);
            _dhcpServer.OnIPReleased.RemoveListener(OnIPReleased);
        }
    }


    void Reset()
    {
        statusLight = GetComponent<Light>();
        if (statusLight == null)
            gameObject.AddComponent<Light>();
    }

    void SetLight(bool isConnected)
    {
        if (statusLight == null)
        {
            Debug.LogWarning("�������� ��������� �� ��������!");
            return;
        }

        statusLight.color = isConnected ? connectedColor : disconnectedColor; // ������������ ��������� �����
        statusLight.intensity = isConnected ? 3f : 1f;
        statusLight.enabled = true;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = isConnected ? Color.cyan : Color.gray;
    }

    string GenerateMAC()
    {
        System.Random rand = new System.Random();
        byte[] macBytes = new byte[6];
        rand.NextBytes(macBytes);
        return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
            macBytes[0], macBytes[1], macBytes[2],
            macBytes[3], macBytes[4], macBytes[5]);
    }
}
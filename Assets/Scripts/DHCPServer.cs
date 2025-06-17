using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class DHCPServer : MonoBehaviour
{
    [SerializeField] private string _subnet = "192.168.1.0/24"; //�������� ���������� 
    [SerializeField] private float _leaseTime = 86400; //����� ������  ��������� � ��������
    [SerializeField] private bool _enableLogging = true; // ���� ��� ���/���� �����

    private HashSet<string> _availableIPs = new HashSet<string>(); //��������� ��������� ����
    private Dictionary<string, DHCPLease> _leasedIPs = new Dictionary<string, DHCPLease>(); //������� ������� ���� (���� - ����, �������� - ������ ������)
    private List<string> _reservedIPs = new List<string>(); //������ ����������������� ����

    public UnityEvent<string> OnIPLeased; //������� ������� ���������� ��� ������ ���������
    public UnityEvent<string> OnIPReleased; //������� ������� ���������� ��� ������������ ���������

    private class DHCPLease
    {
        public string MacAddress; //MAC-����� ����������
        public float ExpiryTime; //����� ��������� ������ (� ��� � ������� �������)
    }
    void Start()
    {
        _availableIPs = new HashSet<string>();
        InitPool(); // ����� ������������� ����

        Debug.Log($"��� ���������������. �������� IP: {_availableIPs.Count}");
    }

    //������������� ���� ip
    public void InitPool()
    {
        _availableIPs.Clear(); //������� ������ ��������� ip
        _leasedIPs.Clear(); //������� ������ ������� Ip

        string baseIP = _subnet.Split('/')[0]; // ��������� �������� ������ ip (�� /)
        string[] octets = baseIP.Split('.');  // ��������� �� ������ (192, 168, 1, 0)

        for (int i = 1; i <= 254; i++)
        {
            string ip = $"{octets[0]}.{octets[1]}.{octets[2]}.{i}";
            _availableIPs.Add(ip); //���������� � ��� ��������� �������
        }
        if (_enableLogging) // ���� ���������� ��������
            Debug.Log($"��� IP ���������������. ��������: {_availableIPs.Count} �������");
    }

    public string RequestIP(string macAddress)
    {
        if (!ValidateMac(macAddress))
        {
            Debug.LogError($"������������ MAC: {macAddress}");
            return null;
        }

        if (_availableIPs.Count == 0)
        {
            CleanExpiredLeases();
            if (_availableIPs.Count == 0)
            {
                Debug.LogError("DHCP Pool is empty!");
                return null;
            }
        }

        // ���������� First() � LINQ
        string ip = _availableIPs.First();

        _leasedIPs[ip] = new DHCPLease
        {
            MacAddress = macAddress,
            ExpiryTime = Time.time + _leaseTime
        };
        _availableIPs.Remove(ip);

        if (_enableLogging)
            Debug.Log($"Assigned IP {ip} to MAC {macAddress}");

        OnIPLeased?.Invoke(ip);
        return ip;
    }
    private void CleanExpiredLeases()
{
    var leasesToCheck = new List<string>(_leasedIPs.Keys);

    foreach (var ip in leasesToCheck)
    {
        if (!string.IsNullOrEmpty(ip) && _leasedIPs.TryGetValue(ip, out var lease))
        {
            if (lease.ExpiryTime <= Time.time)
            {
                _availableIPs.Add(ip);
                _leasedIPs.Remove(ip);
            }
        }
    }
}

public bool ReleaseIP(string ip)
    {
        // �������� �������� �� null � ������ ������
        if (string.IsNullOrEmpty(ip))
        {
            Debug.LogError("������� ������ IP-�����!");
            return false;
        }

        if (!IsValidIP(ip))
        {
            Debug.LogError($"�������� ������ IP: {ip}");
            return false;
        }

        _availableIPs.Add(ip);
        _leasedIPs.Remove(ip);

        if (_enableLogging)
            Debug.Log($"���������� IP: {ip}");

        OnIPReleased?.Invoke(ip);
        return true;
    }

    private bool IsValidIP(string ip)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(ip,
            @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
    }
    public bool RenewLease(string ip, string macAddress)
    {
        if (!_leasedIPs.TryGetValue(ip, out DHCPLease lease))
        {
            Debug.LogWarning($"IP {ip} is not leased!");  // ������� ��������������
            return false;  // ���������� false
        }
        if (lease.MacAddress != macAddress)  // ���� MAC �� ���������
        {
            Debug.LogWarning($"MAC {macAddress} doesen't match leased IP {ip}");
            return false;
        }
        lease.ExpiryTime = Time.time + _leaseTime;
        if (_enableLogging)
            Debug.Log($"Lease renewed for IP {ip}");

        return true;
    
    }
    private bool ValidateMac(string mac)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(
            mac,
            "^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$"
            );
    }
    public List<string> GetLeasedIPs()
    {
        return new List<string>(_leasedIPs.Keys);
    }
}

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

[System.Serializable]
public class IPAddress
{
    [SerializeField] private string address;
    private bool isValid;

    // Конструкторы
    public IPAddress() : this("0.0.0.0") {}
    
    public IPAddress(string ipString)
    {
        isValid = ValidateIP(ipString);
        address = isValid ? ipString : "0.0.0.0";
    }

    // Валидация IP
    private bool ValidateIP(string ip)
    {
        if (string.IsNullOrEmpty(ip)) return false;
        
        // Регулярное выражение для проверки IPv4
        string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        return Regex.IsMatch(ip, pattern);
    }

    // Получение локального IP
    public static string GetLocalIP()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        catch
        {
            return "127.0.0.1";
        }
    }

    // Свойства
    public string Address => address;
    public bool IsValid => isValid;

    // Преобразование в строку
    public override string ToString() => address;

    // Разбиение на октеты
    public byte[] GetOctets()
    {
        string[] parts = address.Split('.');
        byte[] octets = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            octets[i] = byte.Parse(parts[i]);
        }
        return octets;
    }
}
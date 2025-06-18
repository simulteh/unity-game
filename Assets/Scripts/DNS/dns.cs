using UnityEngine;
using System.Net; // Для работы с DNS
using System.Net.Sockets; // Для работы с сетевыми запросами

public class dns : MonoBehaviour
{
    [SerializeField] private string domainName = "google.com"; // Поле для ввода домена в Inspector
    
    void Start()
    {
        // Вызываем метод для получения IP
        string ipAddress = GetIPFromDomain(domainName);
        
        // Выводим результат в консоль Unity
        if (!string.IsNullOrEmpty(ipAddress))
        {
            Debug.Log($"Домен: {domainName} → IP: {ipAddress}");
        }
        else
        {
            Debug.LogError("Не удалось получить IP для указанного домена!");
        }
    }

    // Метод для получения IP по доменному имени
    public string GetIPFromDomain(string domain)
    {
        try
        {
            // Получаем все IP-адреса для домена (может быть несколько)
            IPAddress[] addresses = Dns.GetHostAddresses(domain);
            
            // Берём первый адрес (обычно это IPv4)
            if (addresses.Length > 0)
            {
                return addresses[0].ToString();
            }
        }
        catch (SocketException ex) // Обработка ошибок (если домен не существует)
        {
            Debug.LogError($"Ошибка DNS: {ex.Message}");
        }
        
        return null; // Если что-то пошло не так
    }
}